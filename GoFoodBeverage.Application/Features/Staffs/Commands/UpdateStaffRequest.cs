using System;
using MediatR;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Staff;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;


namespace GoFoodBeverage.Application.Features.Staffs.Commands
{
    public class UpdateStaffRequest : IRequest<bool>
    {
        public EditStaffRequestDto Staff { get; set; }

        public class EditStaffRequestDto
        {
            public Guid Id { get; set; }

            public Guid StaffId { get; set; }

            public string Code { get; set; }

            public string Name { get; set; }

            public string Phone { get; set; }

            public string Email { get; set; }

            public bool Gender { get; set; }

            public DateTime? Birthday { get; set; }
        }

        public List<StaffGroupPermissionBranchRequestModel> GroupPermissionBranches { get; set; }
    }

    public class UpdateStaffByIdRequestHandler : IRequestHandler<UpdateStaffRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateStaffByIdRequestHandler(
             IUnitOfWork unitOfWork,
             IUserProvider userProvider,
             IUserActivityService userActivityService
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateStaffRequest request, CancellationToken cancellationToken)
        {

            // Get the current user information from the user token.
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            // Get the current staff from database.
            var staff = await _unitOfWork.Staffs.GetStaffByIdForEditAsync(request.Staff.Id, loggedUser.StoreId);
            ThrowError.Against(staff == null, "Cannot find staff information");

            await TenantValidationAsync(request, loggedUser.StoreId.Value);

            // Get permission list from the current request.
            var groupPermissionIds = request.
                GroupPermissionBranches.SelectMany(p => p.GroupPermissionIds);

            // Get permission list from database by the id list in the current request.
            var groupPermissions = await _unitOfWork
                .GroupPermissions.Find(p => p.StoreId == loggedUser.StoreId && groupPermissionIds.Any(pid => pid == p.Id)).
                ToListAsync(cancellationToken: cancellationToken);

            // Get a list of branches from the current request.
            var branchIds = request.GroupPermissionBranches.SelectMany(p => p.BranchIds);

            // Get branch list from database by the branch id list from the current request.
            var storeBranches = await _unitOfWork.
                StoreBranches.
                Find(p => p.StoreId == loggedUser.StoreId && branchIds.Any(pid => pid == p.Id)).
                ToListAsync(cancellationToken: cancellationToken);

            /// Get a list of old permissions of current staff, this list will be removed before inserting new data.
            var deleteStaffGroupPermissionBranchItems = _unitOfWork.
                StaffGroupPermissionBranches.
                Find(x => x.StoreId == loggedUser.StoreId && x.StaffId == request.Staff.Id).ToList();

            // Update data for the staff.
            var modifiedStaff = UpdateStaff(staff, request);

            var staffGroupPermissionBranches =
                UpdateStaffGroupPermissionBranches(
                    request.GroupPermissionBranches,
                    groupPermissions,
                    storeBranches,
                    modifiedStaff,
                    loggedUser.StoreId.Value
                );

            // Create a new transaction to save data more securely, data will be restored if an error occurs.
            using var updateStaffTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Remove old data from the database.
                await _unitOfWork.StaffGroupPermissionBranches.RemoveRangeAsync(deleteStaffGroupPermissionBranchItems);
                await _unitOfWork.Staffs.UpdateAsync(modifiedStaff);

                // Add new data.
                await _unitOfWork.StaffGroupPermissionBranches.AddRangeAsync(staffGroupPermissionBranches);
                await _unitOfWork.SaveChangesAsync();

                // Complete this transaction, data will be saved.
                await updateStaffTransaction.CommitAsync(cancellationToken);

                // Add a new user activity.
                await _userActivityService.LogAsync(request);
            }
            catch (Exception)
            {
                // Data will be restored.
                await updateStaffTransaction.RollbackAsync(cancellationToken);

                return false;
            }

            return true;
        }

        /// <summary>
        /// This method is used to check valid data.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <param name="storeId">User's current store.</param>
        /// <returns></returns>
        private async Task TenantValidationAsync(UpdateStaffRequest request, Guid storeId)
        {
            var staffCodeExisted = await _unitOfWork.Staffs.CheckExistStaffCodeInStoreAsync(request.Staff.Id, request.Staff.Code, storeId);
            ThrowError.Against(staffCodeExisted != null, new JObject()
            {
                { $"{nameof(request.Staff.Code)}", "Staff code has existed" },
            });
        }

        /// <summary>
        /// This method is used to update the staff information.
        /// </summary>
        /// <param name="currentStaff">The current staff</param>
        /// <param name="request">Data from the request.</param>
        /// <returns></returns>
        private Staff UpdateStaff(Staff currentStaff, UpdateStaffRequest request)
        {
            currentStaff.Code = request.Staff.Code;
            currentStaff.FullName = request.Staff.Name;
            currentStaff.Gender = request.Staff.Gender;
            currentStaff.Birthday = request.Staff.Birthday;

            return currentStaff;
        }

        /// <summary>
        /// This method is used to build data to add data to the database.
        /// </summary>
        /// <param name="staffGroupPermissionBranchesRequest">A list of permissions from the request.</param>
        /// <param name="groupPermissions">A list of permissions from the database filtered by request data.</param>
        /// <param name="storeBranches">A list of branches from the database filtered by request data.</param>
        /// <param name="staff">The current staff.</param>
        /// <returns></returns>
        private List<StaffGroupPermissionBranch> UpdateStaffGroupPermissionBranches(
            List<StaffGroupPermissionBranchRequestModel> staffGroupPermissionBranchesRequest,
            List<GroupPermission> groupPermissions,
            List<StoreBranch> storeBranches,
            Staff staff,
            Guid? storeId
        )
        {
            var staffGroupPermissionBranches = new List<StaffGroupPermissionBranch>();

            // Go to each item and add permission for this staff.
            staffGroupPermissionBranchesRequest.ForEach(requestModel =>
            {
                var groups = groupPermissions.Where(g => requestModel.GroupPermissionIds.Any(gid => gid == g.Id));
                if (!groups.Any()) return;

                // Add permissions for each branch.
                foreach (var branchId in requestModel.BranchIds)
                {
                    var branch = storeBranches.FirstOrDefault(s => s.Id == branchId);
                    if (branch == null) return;

                    // Create a new permission object and add it to the list.
                    var staffGroupPermissionBranch = UpdateStaffGroupPermissionBranch(staff.Id, groups, branch.Id, storeId);
                    staffGroupPermissionBranches.Add(staffGroupPermissionBranch);
                }
            });

            return staffGroupPermissionBranches;
        }

        /// <summary>
        /// This method is used to update the permissions for branch.
        /// </summary>
        /// <param name="staffId">The current staff.</param>
        /// <param name="groupPermissions">A list of permissions.</param>
        /// <param name="branchId">The branch id, for example: b1ad8c25-adf1-4e35-a164-410b631bbc73</param>
        /// <returns></returns>
        private StaffGroupPermissionBranch UpdateStaffGroupPermissionBranch(
            Guid staffId,
            IEnumerable<GroupPermission> groupPermissions,
            Guid? branchId,
            Guid? storeId
        )
        {
            var staffGroupPermissionBranch = new StaffGroupPermissionBranch()
            {
                StaffId = staffId,
                StoreId = storeId,
                GroupPermissionBranches = new List<GroupPermissionBranch>()
            };

            foreach (var groupPermission in groupPermissions)
            {
                var groupPermissionBranch = new GroupPermissionBranch()
                {
                    StoreBranchId = branchId,
                    GroupPermissionId = groupPermission.Id,
                    IsApplyAllBranch = branchId == null,
                    StoreId = storeId
                };
                staffGroupPermissionBranch.GroupPermissionBranches.Add(groupPermissionBranch);
            }

            return staffGroupPermissionBranch;
        }
    }
}
