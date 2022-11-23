using MediatR;
using MoreLinq;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Staff;
using GoFoodBeverage.Models.Store;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Permission;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using System;

namespace GoFoodBeverage.Application.Features.Staffs.Queries
{
    public class GetStaffsRequest : IRequest<GetStaffsResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetStaffsResponse
    {
        public IEnumerable<StaffModel> Staffs { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }


    public class GetStaffsRequestHandler : IRequestHandler<GetStaffsRequest, GetStaffsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;

        public GetStaffsRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
        }

        public async Task<GetStaffsResponse> Handle(GetStaffsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var staffsInStore = new PagingExtensions.Pager<Staff>(new List<Staff>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                staffsInStore = await _unitOfWork.Staffs
                                   .GetAllStaffInStore(loggedUser.StoreId.Value)
                                   .Include(s => s.StaffGroupPermissionBranchs)
                                   .ThenInclude(gpb => gpb.GroupPermissionBranches)
                                   .ThenInclude(gp => gp.GroupPermission)
                                   .Include(s => s.StaffGroupPermissionBranchs)
                                   .ThenInclude(gpb => gpb.GroupPermissionBranches)
                                   .ThenInclude(gp => gp.StoreBranch)
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }
            else
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                staffsInStore = await _unitOfWork.Staffs
                                   .GetAllStaffInStore(loggedUser.StoreId.Value)
                                   .Where(s => s.FullName.ToLower().Contains(keySearch) || s.PhoneNumber.ToLower().Contains(keySearch))
                                   .Include(s => s.StaffGroupPermissionBranchs)
                                   .ThenInclude(gpb => gpb.GroupPermissionBranches)
                                   .ThenInclude(gp => gp.GroupPermission)
                                   .Include(s => s.StaffGroupPermissionBranchs)
                                   .ThenInclude(gpb => gpb.GroupPermissionBranches)
                                   .ThenInclude(gp => gp.StoreBranch)
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var staffsResponse = await GetStaffModelAsync(staffsInStore.Result.ToList(), request, loggedUser.StoreId);
            return new GetStaffsResponse()
            {
                Staffs = staffsResponse,
                PageNumber = request.PageNumber,
                Total = staffsInStore.Total
            };
        }

        private async Task<List<StaffModel>> GetStaffModelAsync(List<Staff> staffs, GetStaffsRequest request, Guid? storeId)
        {
            var staffsResponse = new List<StaffModel>();
            var staffIds = staffs.Select(s => s.Id);
            var staffGroupPermissionBranches = await _unitOfWork.StaffGroupPermissionBranches
                .GetStaffGroupPermissionBranchesByStaffIds(staffIds, storeId)
                .AsNoTracking()
                .ToListAsync();

            var allBranches = await _unitOfWork.StoreBranches.GetStoreBranchesByStoreId(storeId).AsNoTracking().ToListAsync();
            staffs.ForEach(staff =>
            {
                var index = staffs.IndexOf(staff) + ((request.PageNumber - 1) * request.PageSize) + 1;
                var staffGroupPermissionBranchesByStaff = staffGroupPermissionBranches.Where(i => i.StaffId == staff.Id);

                var branches = allBranches;
                var isNotApplyAllBranches = staffGroupPermissionBranchesByStaff.SelectMany(s => s.GroupPermissionBranches).All(i => i.IsApplyAllBranch == false);
                if (isNotApplyAllBranches)
                {
                    branches = staffGroupPermissionBranchesByStaff.SelectMany(s => s.GroupPermissionBranches.Where(g => g.IsApplyAllBranch == false).Select(g => g.StoreBranch)).DistinctBy(b => b.Id).ToList();
                }

                var branchesResponse = _mapper.Map<IEnumerable<StoreBranchModel>>(branches);
                var groups = staffGroupPermissionBranchesByStaff.SelectMany(s => s.GroupPermissionBranches.Select(g => g.GroupPermission)).DistinctBy(g => g.Id);
                var groupsResponse = _mapper.Map<IEnumerable<GroupPermissionModel>>(groups);

                var staffModel = new StaffModel()
                {
                    Id = staff.Id,
                    No = index,
                    FullName = staff.FullName,
                    PhoneNumber = staff.PhoneNumber,
                    StoreBranches = branchesResponse,
                    GroupPermissions = groupsResponse
                };

                staffsResponse.Add(staffModel);
            });

            return staffsResponse;
        }
    }
}
