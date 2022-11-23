using AutoMapper;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Permission;
using GoFoodBeverage.Models.Staff;
using GoFoodBeverage.Models.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Staffs.Queries
{
    public class GetDataStaffManagementRequest : IRequest<GetDataStaffManagementResponse>
    {
        public int ScreenKey { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }

        public Guid? GroupPermissionId { get; set; }

        public Guid? BranchId { get; set; }
    }

    public class GetDataStaffManagementResponse
    {
        public IEnumerable<StaffModel> Staffs { get; set; }

        public IEnumerable<GroupPermissionModel> GroupPermissions { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }

        public List<TabRecordDto> TabRecords { get; set; }

        public class TabRecordDto
        {
            public int ScreenKey { get; set; }

            public int TotalRecords { get; set; }
        }
    }


    public class GetDataStaffManagementRequestHandler : IRequestHandler<GetDataStaffManagementRequest, GetDataStaffManagementResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        private const int tabStaff = 1;
        private const int tabPermissionGroup = 2;

        public GetDataStaffManagementRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetDataStaffManagementResponse> Handle(GetDataStaffManagementRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageNumber = 20;
            var response = new GetDataStaffManagementResponse();
            switch (request.ScreenKey)
            {
                default:
                case tabStaff:
                    var staffsInStore = await GetListStaffAsync(request, loggedUser);
                    var staffsResponse = await GetStaffModelAsync(staffsInStore.Result.ToList(), request, loggedUser.StoreId);
                    response.Staffs = staffsResponse;
                    response.PageNumber = request.PageNumber;
                    response.Total = staffsInStore.Total;
                    break;
                case tabPermissionGroup:
                    var groupPermissionsByPaging = await GetListGroupPermissionsAsync(request, loggedUser, cancellationToken);
                    var groupPermissionModels = _mapper.Map<IEnumerable<GroupPermissionModel>>(groupPermissionsByPaging.Result);
                    response.GroupPermissions = groupPermissionModels;
                    response.Total = groupPermissionsByPaging.Total;
                    break;
            }

            response.TabRecords = await GetTabRecordsAsync(request, loggedUser);

            return response;
        }

        private async Task<PagingExtensions.Pager<Staff>> GetListStaffAsync(GetDataStaffManagementRequest request, LoggedUserModel loggedUser)
        {
            var staffsInStore = new PagingExtensions.Pager<Staff>(new List<Staff>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                if (request.GroupPermissionId.HasValue && request.BranchId.HasValue)
                {
                    // Staff by group permission id and branch id
                    staffsInStore = await _unitOfWork.GroupPermissionBranches
                    .Find(gpb => gpb.StoreId == loggedUser.StoreId && gpb.StoreBranchId == request.BranchId && gpb.GroupPermissionId == request.GroupPermissionId)
                    .Include(gpb => gpb.GroupPermission)
                    .Include(gpb => gpb.StaffGroupPermissionBranch)
                    .ThenInclude(sgpb => sgpb.Staff)
                    .AsNoTracking()
                    .Select(g => g.StaffGroupPermissionBranch.Staff).OrderByDescending(p => p.CreatedTime)
                    .ToPaginationAsync(request.PageNumber, request.PageSize);
                } else if (request.GroupPermissionId.HasValue)
                {
                    // Staff by group permission id
                    staffsInStore = await _unitOfWork.GroupPermissionBranches
                    .Find(gpb => gpb.StoreId == loggedUser.StoreId && gpb.GroupPermissionId == request.GroupPermissionId)
                    .Include(gpb => gpb.GroupPermission)
                    .Include(gpb => gpb.StaffGroupPermissionBranch)
                    .ThenInclude(sgpb => sgpb.Staff)
                    .AsNoTracking()
                    .Select(g => g.StaffGroupPermissionBranch.Staff).OrderByDescending(p => p.CreatedTime)
                    .ToPaginationAsync(request.PageNumber, request.PageSize);
                }
                else if (request.BranchId.HasValue)
                {
                    // Staff by group branch id
                    staffsInStore = await _unitOfWork.GroupPermissionBranches
                    .Find(gpb => gpb.StoreId == loggedUser.StoreId && gpb.StoreBranchId == request.BranchId)
                    .Include(gpb => gpb.GroupPermission)
                    .Include(gpb => gpb.StaffGroupPermissionBranch)
                    .ThenInclude(sgpb => sgpb.Staff)
                    .AsNoTracking()
                    .Select(g => g.StaffGroupPermissionBranch.Staff)
                    .OrderByDescending(p => p.CreatedTime)
                    .Distinct()
                    .ToPaginationAsync(request.PageNumber, request.PageSize);
                }
                else
                {
                    staffsInStore = await _unitOfWork.Staffs
                                   .GetAllStaffInStore(loggedUser.StoreId.Value)
                                   .Include(s => s.StaffGroupPermissionBranchs)
                                   .ThenInclude(gpb => gpb.GroupPermissionBranches)
                                   .ThenInclude(gp => gp.GroupPermission).Where(group => request.GroupPermissionId.HasValue ? group.Id == request.GroupPermissionId.Value : true)
                                   .Include(s => s.StaffGroupPermissionBranchs)
                                   .ThenInclude(gpb => gpb.GroupPermissionBranches)
                                   .ThenInclude(gp => gp.StoreBranch).Where(branch => request.BranchId.HasValue ? branch.Id == request.BranchId.Value : true)
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
                }
            }
            else
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                if (request.GroupPermissionId.HasValue && request.BranchId.HasValue)
                {
                    // Staff by group permission id and branch id
                    staffsInStore = await _unitOfWork.GroupPermissionBranches
                    .Find(gpb => gpb.StoreId == loggedUser.StoreId && gpb.GroupPermissionId == request.GroupPermissionId && gpb.StoreBranchId == request.BranchId)
                    .Include(gpb => gpb.GroupPermission)
                    .Include(gpb => gpb.StaffGroupPermissionBranch)
                    .ThenInclude(sgpb => sgpb.Staff)
                    .AsNoTracking()
                    .Select(g => g.StaffGroupPermissionBranch.Staff).Where(s => s.FullName.ToLower().Contains(keySearch) || s.PhoneNumber.ToLower().Contains(keySearch)).OrderByDescending(p => p.CreatedTime)
                    .ToPaginationAsync(request.PageNumber, request.PageSize);
                }
                else if (request.GroupPermissionId.HasValue)
                {
                    // Staff by group permission id
                    staffsInStore = await _unitOfWork.GroupPermissionBranches
                    .Find(gpb => gpb.StoreId == loggedUser.StoreId && gpb.GroupPermissionId == request.GroupPermissionId)
                    .Include(gpb => gpb.GroupPermission)
                    .Include(gpb => gpb.StaffGroupPermissionBranch)
                    .ThenInclude(sgpb => sgpb.Staff)
                    .AsNoTracking()
                    .Select(g => g.StaffGroupPermissionBranch.Staff).Where(s => s.FullName.ToLower().Contains(keySearch) || s.PhoneNumber.ToLower().Contains(keySearch)).OrderByDescending(p => p.CreatedTime)
                    .ToPaginationAsync(request.PageNumber, request.PageSize);
                }
                else if (request.BranchId.HasValue)
                {
                    // Staff by group branch id
                    staffsInStore = await _unitOfWork.GroupPermissionBranches
                    .Find(gpb => gpb.StoreId == loggedUser.StoreId && gpb.StoreBranchId == request.BranchId)
                    .Include(gpb => gpb.GroupPermission)
                    .Include(gpb => gpb.StaffGroupPermissionBranch)
                    .ThenInclude(sgpb => sgpb.Staff)
                    .AsNoTracking()
                    .Select(g => g.StaffGroupPermissionBranch.Staff).Where(s => s.FullName.ToLower().Contains(keySearch) || s.PhoneNumber.ToLower().Contains(keySearch)).OrderByDescending(p => p.CreatedTime)
                    .ToPaginationAsync(request.PageNumber, request.PageSize);
                }
                else
                {
                    staffsInStore = await _unitOfWork.Staffs
                                   .GetAllStaffInStore(loggedUser.StoreId.Value)
                                   .Where(s => s.FullName.ToLower().Contains(keySearch) || s.PhoneNumber.ToLower().Contains(keySearch))
                                   .Include(s => s.StaffGroupPermissionBranchs)
                                   .ThenInclude(gpb => gpb.GroupPermissionBranches)
                                   .ThenInclude(gp => gp.GroupPermission).Where(group => request.GroupPermissionId.HasValue ? group.Id == request.GroupPermissionId.Value : true)
                                   .Include(s => s.StaffGroupPermissionBranchs)
                                   .ThenInclude(gpb => gpb.GroupPermissionBranches)
                                   .ThenInclude(gp => gp.StoreBranch).Where(branch => request.BranchId.HasValue ? branch.Id == request.BranchId.Value : true)
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
                }
            }

            return staffsInStore;
        }

        private async Task<List<StaffModel>> GetStaffModelAsync(List<Staff> staffs, GetDataStaffManagementRequest request, Guid? storeId)
        {
            var staffsResponse = new List<StaffModel>();
            var staffIds = staffs.Select(s => s.Id);
            var staffGroupPermissionBranches = await _unitOfWork.StaffGroupPermissionBranches
                .GetStaffGroupPermissionBranchesByStaffIds(staffIds, storeId)
                .AsNoTracking()
                .ToListAsync();

            var allBranches = await _unitOfWork.StoreBranches.GetStoreBranchesByStoreId(storeId).AsNoTracking().ToListAsync();
            var stores = await _unitOfWork.Stores.GetAll().AsNoTracking().ToListAsync();

            staffs.ForEach(staff =>
            {
                var storeExisted = stores.Any(x => x.InitialStoreAccountId == staff.AccountId);
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
                    GroupPermissions = groupsResponse,
                    IsInitialStoreAccount = storeExisted
                };

                staffsResponse.Add(staffModel);
            });

            return staffsResponse;
        }

        private async Task<PagingExtensions.Pager<GroupPermissionModel>> GetListGroupPermissionsAsync(GetDataStaffManagementRequest request, LoggedUserModel loggedUser, CancellationToken cancellationToken)
        {
            var groupPermissions = await _unitOfWork.GroupPermissions
                .GetGroupPermissionsByStoreId(loggedUser.StoreId)
                .Include(gp => gp.GroupPermissionBranches)
                .ThenInclude(gpb => gpb.StaffGroupPermissionBranch)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ToListAsync(cancellationToken: cancellationToken);

            var groupPermissionsResponse = _mapper.Map<List<GroupPermissionModel>>(groupPermissions);

            var listStaffId = groupPermissionsResponse.Select(g => g.CreatedByStaffId).ToList();
            var listStaff = await _unitOfWork.Staffs.GetAllStaffByListStaffId(listStaffId).ToListAsync(cancellationToken: cancellationToken);
            foreach (var groupPermission in groupPermissionsResponse)
            {
                var staff = listStaff.FirstOrDefault(s => s.Id == groupPermission.CreatedByStaffId);
                var groupPermissionById = groupPermissions.FirstOrDefault(s => s.Id == groupPermission.Id);
                var staffs = groupPermissionById.GroupPermissionBranches.Select(gpb => gpb.StaffGroupPermissionBranch.StaffId).Distinct();
                groupPermission.CreatedByStaffName = staff.FullName;
                groupPermission.NumberOfMember = staffs.Count();
            }

            if (!string.IsNullOrEmpty(request.KeySearch) && groupPermissionsResponse != null)
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                groupPermissionsResponse = groupPermissionsResponse.Where(g => g.Name.ToLower().Contains(keySearch)
                || g.CreatedByStaffName.ToLower().Contains(keySearch)).ToList();
            }

            var groupPermissionsByPaging = groupPermissionsResponse.ToPagination(request.PageNumber, request.PageSize);

            return groupPermissionsByPaging;
        }


        private async Task<List<GetDataStaffManagementResponse.TabRecordDto>> GetTabRecordsAsync(GetDataStaffManagementRequest request, LoggedUserModel loggedUser)
        {
            var result = new List<GetDataStaffManagementResponse.TabRecordDto>();
            var totalStaffsInStore = await _unitOfWork.Staffs.GetAllStaffInStore(loggedUser.StoreId.Value).AsNoTracking().CountAsync();
            var totalGroupPermissionsInStore = await _unitOfWork.GroupPermissions.GetGroupPermissionsByStoreId(loggedUser.StoreId.Value).AsNoTracking().CountAsync();

            result.Add(new GetDataStaffManagementResponse.TabRecordDto()
            {
                ScreenKey = tabStaff,
                TotalRecords = totalStaffsInStore,
            });

            result.Add(new GetDataStaffManagementResponse.TabRecordDto()
            {
                ScreenKey = tabPermissionGroup,
                TotalRecords = totalGroupPermissionsInStore,
            });

            return result;

        }
    }
}
