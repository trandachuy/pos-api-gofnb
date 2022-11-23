using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Permission;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace GoFoodBeverage.Application.Features.Settings.Queries
{
    public class GetPermissionGroupsRequest : IRequest<GetPermissionGroupsResponse> { }

    public class GetPermissionGroupsResponse
    {
        public IEnumerable<PermissionGroupModel> PermissionGroups { get; set; }
    }

    public class GetPermissionGroupsRequestHandler : IRequestHandler<GetPermissionGroupsRequest, GetPermissionGroupsResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetPermissionGroupsRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetPermissionGroupsResponse> Handle(GetPermissionGroupsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var packageId = (from tblOPackage in _unitOfWork.OrderPackages.GetAll()
                             where tblOPackage.Id == (from tblStore in _unitOfWork.Stores.GetAll()
                                                      where tblStore.Id == loggedUser.StoreId
                                                      select tblStore.ActivatedByOrderPackageId.Value).Single()
                             select tblOPackage.PackageId)
                             .Single();

            var permissions = await _unitOfWork.Packages
                .Find(p => p.Id == packageId)
                .Include(p => p.PackageFunctions)
                .ThenInclude(pf => pf.Function)
                .ThenInclude(f => f.FunctionPermissions).ThenInclude(fp => fp.Permission).ThenInclude(p => p.PermissionGroup)
                .AsNoTracking()
                .SelectMany(p => p.PackageFunctions.SelectMany(pf => pf.Function.FunctionPermissions.Select(fp => fp.Permission)))
                .ToListAsync(cancellationToken: cancellationToken);

            var permissionGroups = permissions
                .Select(p => p.PermissionGroup)
                .DistinctBy(pg => pg.Id)
                .OrderBy(x => x.Order)
                .ToList();

            permissionGroups.ForEach(pg =>
            {
                pg.Permissions = permissions.Where(p => p.PermissionGroupId == pg.Id).ToList();
            });

            var permissionGroupsModel = _mapper.Map<List<PermissionGroupModel>>(permissionGroups);
            var response = new GetPermissionGroupsResponse()
            {
                PermissionGroups = permissionGroupsModel
            };

            return response;
        }
    }
}
