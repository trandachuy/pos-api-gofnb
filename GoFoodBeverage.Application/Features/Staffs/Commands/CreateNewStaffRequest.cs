using System;
using MediatR;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Reflection;
using Newtonsoft.Json.Linq;
using GoFoodBeverage.Email;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Staff;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Settings;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Providers.Email;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Helpers;

namespace GoFoodBeverage.Application.Features.Staffs.Commands
{
    public class CreateNewStaffRequest : IRequest<bool>
    {
        public CreateNewStaffRequestModel Staff { get; set; }

        public List<StaffGroupPermissionBranchRequestModel> GroupPermissionBranches { get; set; }
    }

    public class CreateNewStaffRequestHandler : IRequestHandler<CreateNewStaffRequest, bool>
    {
        private readonly DomainFE _domainFE;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;
        private readonly IEmailSenderProvider _emailProvider;

        public CreateNewStaffRequestHandler(
             IUnitOfWork unitOfWork,
             IUserProvider userProvider,
             IOptions<DomainFE> domainFE,
             IUserActivityService userActivityService,
             IEmailSenderProvider emailProvider
        )
        {
            _unitOfWork = unitOfWork;
            _domainFE = domainFE.Value;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
            _emailProvider = emailProvider;
        }

        /// <summary>
        /// This method is used to handle data from the request.
        /// </summary>
        /// <param name="request">Data attached from the current request.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<bool> Handle(CreateNewStaffRequest request, CancellationToken cancellationToken)
        {

            // Get the current user information from the user token.
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            // Get the current store by the id.
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);

            // Check valid data.
            ThrowError.Against(store == null, "Cannot find store information");
            await TenantValidationAsync(request, store.Id);

            /// Get permission list from the current request.
            var groupPermissionIds = request.GroupPermissionBranches.SelectMany(p => p.GroupPermissionIds);

            // Get permission list from database by the id list in the current request.
            var groupPermissions = await _unitOfWork
                .GroupPermissions
                .Find(p => p.StoreId == loggedUser.StoreId && groupPermissionIds.Any(pid => pid == p.Id))
                .ToListAsync(cancellationToken: cancellationToken);

            // Get a list of branches from the current request.
            var branchIds = request.GroupPermissionBranches.SelectMany(p => p.BranchIds);

            // Get branch list from database by the branch id list from the current request.
            var storeBranches = await _unitOfWork.
                StoreBranches.
                Find(p => p.StoreId == loggedUser.StoreId && branchIds.Any(pid => pid == p.Id)).
                ToListAsync(cancellationToken: cancellationToken);

            // Generate the user's password.
            var password = StringHelpers.GeneratePassword();

            // Set data for the staff to add to the database.
            var newStaff = await CreateStaffAndAccountAsync(request.Staff, store, password, cancellationToken);

            // Create permission for the current staff.
            var staffGroupPermissionBranches = CreateStaffGroupPermissionBranches(request.GroupPermissionBranches, groupPermissions, storeBranches, newStaff, loggedUser.StoreId.Value);

            // Create a new transaction to save data more securely, data will be restored if an error occurs.
            using var createStaffTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Add a new staff to the database.
                await _unitOfWork.Staffs.AddAsync(newStaff);

                // Add permission list for the current staff.
                await _unitOfWork.StaffGroupPermissionBranches.AddRangeAsync(staffGroupPermissionBranches);
                await _unitOfWork.SaveChangesAsync();

                // Complete this transaction, data will be saved.
                await createStaffTransaction.CommitAsync(cancellationToken);

                // Send email to user.
                await SendEmailPasswordAsync(newStaff.FullName, newStaff.Account.Username, password);

                // Add a new user activity.
                await _userActivityService.LogAsync(request);
            }
            catch
            {
                // Data will be restored.
                await createStaffTransaction.RollbackAsync(cancellationToken);

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
        private async Task TenantValidationAsync(CreateNewStaffRequest request, Guid storeId)
        {
            // Staff code unique inside tenant
            var staffCodeExisted = await _unitOfWork.Staffs.GetAllStaffInStore(storeId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code.Trim().ToLower() == request.Staff.Code.Trim().ToLower());
            ThrowError.Against(staffCodeExisted != null, new JObject()
            {
                // Error messages is store at en.json, vi.json
                { $"{nameof(request.Staff.Code)}", "staffManagement.error.staffCodeExisted" },
            });

            // Staff phone unique inside tenant
            var staffPhoneExisted = await _unitOfWork.Staffs.GetAllStaffInStore(storeId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.PhoneNumber == request.Staff.Phone);
            ThrowError.Against(staffPhoneExisted != null, new JObject()
            {
                { $"{nameof(request.Staff.Phone)}", "staffManagement.error.phoneNumberExisted" },
            });

            // Staff email unique inside tenant
            var staffEmailExisted = await _unitOfWork.Staffs.GetAllStaffInStore(storeId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == request.Staff.Email);
            ThrowError.Against(staffEmailExisted != null, new JObject()
            {
                { $"{nameof(request.Staff.Email)}", "staffManagement.error.EmailExisted" },
            });
        }

        /// <summary>
        /// This method is used to create a new staff object to save to the database.
        /// </summary>
        /// <param name="staffModel">Data from the current request.</param>
        /// <param name="store">The current store.</param>
        /// <param name="password">User's password string.</param>
        /// <param name="cancellationToken">Current thread.</param>
        /// <returns></returns>
        private async Task<Staff> CreateStaffAndAccountAsync(
            CreateNewStaffRequestModel staffModel,
            Store store,
            string password,
            CancellationToken cancellationToken)
        {
            ThrowError.Against(!staffModel.Email.IsValidEmail(), "staffManagement.error.EmailInvalid");

            var validateCode = StringHelpers.GenerateValidateCode();
            var passwordHash = (new PasswordHasher<Domain.Entities.Account>()).HashPassword(null, password);
            var staffAccountType = await _unitOfWork.AccountTypes
                .Find(s => s.EnumValue == (int)EnumAccountType.Staff)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            var newStaffAccount = new Domain.Entities.Account()
            {
                Username = staffModel.Email,
                Password = passwordHash,
                EmailConfirmed = true, /// bypass email confirm, will be remove in the feature
                ValidateCode = validateCode,
                AccountTypeId = staffAccountType.Id
            };

            var staff = new Staff()
            {
                StoreId = store.Id,
                Account = newStaffAccount,
                Code = staffModel.Code,
                FullName = staffModel.Name,
                PhoneNumber = staffModel.Phone,
                Email = staffModel.Email,
                Birthday = staffModel.Birthday,
                Gender = staffModel.Gender
            };

            return staff;
        }

        /// <summary>
        /// This method is used to build data to add data to the database.
        /// </summary>
        /// <param name="staffGroupPermissionBranchesRequest">A list of permissions from the request.</param>
        /// <param name="groupPermissions">A list of permissions from the database filtered by request data.</param>
        /// <param name="storeBranches">A list of branches from the database filtered by request data.</param>
        /// <param name="staff">The current staff.</param>
        /// <returns></returns>
        private List<StaffGroupPermissionBranch> CreateStaffGroupPermissionBranches(
            List<StaffGroupPermissionBranchRequestModel> staffGroupPermissionBranchesRequest,
            List<GroupPermission> groupPermissions,
            List<StoreBranch> storeBranches,
            Staff staff,
            Guid? storeId)
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
                    var staffGroupPermissionBranch = CreateStaffGroupPermissionBranch(staff.Id, groups, branch.Id, storeId);
                    staffGroupPermissionBranches.Add(staffGroupPermissionBranch);
                }
            });

            return staffGroupPermissionBranches;
        }

        /// <summary>
        /// This method is used to create object's the class StaffGroupPermissionBranch.
        /// </summary>
        /// <param name="staffId">The current staff id, for example: 92e9bcf5-66d1-4b5e-893c-358e55dcf4df</param>
        /// <param name="groupPermissions">Group list</param>
        /// <param name="branchId">Branch id, for example: 92e9bcf5-66d1-4b5e-893c-358e55dcf4df</param>
        /// <returns></returns>
        private StaffGroupPermissionBranch CreateStaffGroupPermissionBranch(Guid staffId, IEnumerable<GroupPermission> groupPermissions, Guid? branchId, Guid? storeId)
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
                    StoreId = storeId,
                    GroupPermissionId = groupPermission.Id,
                    IsApplyAllBranch = branchId == null,
                };
                staffGroupPermissionBranch.GroupPermissionBranches.Add(groupPermissionBranch);
            }

            return staffGroupPermissionBranch;
        }

        /// <summary>
        /// This method is used to send a email to the current staff when the data has been saved successfully.
        /// </summary>
        /// <param name="emailAddress">Staff's email, for example: staff001@gmail.com</param>
        /// <param name="password">Staff's temporary password.</param>
        /// <returns></returns>
        private async Task<bool> SendEmailPasswordAsync(string fullName, string emailAddress, string password)
        {
            try
            {
                ResourceManager myManager = new("GoFoodBeverage.Application.Providers.Email.EmailTemplate", Assembly.GetExecutingAssembly());
                var link = $"{_domainFE.EndUser}/login?username={emailAddress?.Trim()}";
                string subject = "Welcome to Go Food and Beverage";

                string htmlFromResource = myManager.GetString(EmailTemplates.REGISTER_NEW_STORE_ACCOUNT);
                string htmlContext = string.Format(htmlFromResource,
                                    DefaultConstants.SYSTEM_NAME,
                                    fullName,
                                    emailAddress,
                                    password,
                                    link);

                return await _emailProvider.SendEmailAsync(subject, htmlContext, emailAddress);
            }
            catch
            {
                return false;
            }
        }
    }
}
