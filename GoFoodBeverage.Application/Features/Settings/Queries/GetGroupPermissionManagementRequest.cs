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
    public class GetGroupPermissionManagementRequest : IRequest<GetGroupPermissionManagementResponse>
    {
    }

    public class GetGroupPermissionManagementResponse
    {
        public IEnumerable<GroupPermissionModel> GroupPermissions { get; set; }
    }

    public class GetGroupPermissionManagementRequestHandler : IRequestHandler<GetGroupPermissionManagementRequest, GetGroupPermissionManagementResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;

        public GetGroupPermissionManagementRequestHandler(
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
            _userProvider = userProvider;
        }

        public async Task<GetGroupPermissionManagementResponse> Handle(GetGroupPermissionManagementRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var groupPermissions = await _unitOfWork.GroupPermissions
                .GetGroupPermissionsByStoreId(loggedUser.StoreId)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ProjectTo<GroupPermissionModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetGroupPermissionManagementResponse()
            {
                GroupPermissions = groupPermissions
            };

            return response;
        }
    }
}
