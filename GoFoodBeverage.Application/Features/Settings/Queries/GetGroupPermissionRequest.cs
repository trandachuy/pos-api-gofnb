using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Permission;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Settings.Queries
{
    public class GetGroupPermissionRequest : IRequest<GetGroupPermissionResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetGroupPermissionResponse
    {
        public IEnumerable<GroupPermissionModel> GroupPermissions { get; set; }

        public int Total { get; set; }
    }

    public class GetGroupPermissionRequestHandler : IRequestHandler<GetGroupPermissionRequest, GetGroupPermissionResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;

        public GetGroupPermissionRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
            _userProvider = userProvider;
        }

        public async Task<GetGroupPermissionResponse> Handle(GetGroupPermissionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var groupPermissions = await _unitOfWork.GroupPermissions
                .GetGroupPermissionsByStoreId(loggedUser.StoreId)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ProjectTo<GroupPermissionModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var listStaffId = groupPermissions.Select(g => g.CreatedByStaffId).ToList();
            var listStaff = await _unitOfWork.Staffs.GetAllStaffByListStaffId(listStaffId).ToListAsync(cancellationToken: cancellationToken);
            foreach (var groupPermission in groupPermissions)
            {
                var staff = listStaff.FirstOrDefault(s => s.Id == groupPermission.CreatedByStaffId);
                groupPermission.CreatedByStaffName = staff.FullName;
            }

            if (!string.IsNullOrEmpty(request.KeySearch) && groupPermissions != null)
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                groupPermissions = groupPermissions.Where(g => g.Name.ToLower().Contains(keySearch)
                || g.CreatedByStaffName.ToLower().Contains(keySearch)).ToList();
            }

            var groupPermissionsByPaging = groupPermissions.ToPagination(request.PageNumber, request.PageSize);
            var groupPermissionModels = _mapper.Map<IEnumerable<GroupPermissionModel>>(groupPermissionsByPaging.Result);

            var response = new GetGroupPermissionResponse()
            {
                GroupPermissions = groupPermissionModels,
                Total = groupPermissions.Count
            };

            return response;
        }
    }
}
