using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Staff;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Staffs.Queries
{
    public class GetStaffByIdRequest : IRequest<GetStaffByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetStaffByIdResponse
    {
        public StaffByIdModel Staff { get; set; }
    }

    public class GetStaffByIdRequestHandler : IRequestHandler<GetStaffByIdRequest, GetStaffByIdResponse>
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetStaffByIdRequestHandler(IMapper mapper, IUnitOfWork unitOfWork, IUserProvider userProvider)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        /// <summary>
        /// This method is used to handle the current request.
        /// </summary>
        /// <param name="request">Data has been defined in the model.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<GetStaffByIdResponse> Handle(GetStaffByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            // Get the staff by the id.
            var staffData = await _unitOfWork.Staffs.GetStaffByIdForEditAsync(request.Id.Value, loggedUser.StoreId);
            ThrowError.Against(staffData == null, "Cannot find staff information");

            // Map and set value.
            var staff = _mapper.Map<StaffByIdModel>(staffData);
            staff.StaffId = staff.Id;
            staff.Name = staffData.FullName;
            staff.Code = staffData.Code;
            staff.Phone = staffData.PhoneNumber;
            staff.Email = staffData.Email;
            staff.Gender = staffData.Gender;
            staff.Birthday = staffData.Birthday;

            // Filter necessary data to handle.
            var groupsBeforeHandling = staffData.StaffGroupPermissionBranchs
                .Select(x => new StaffByIdModel.GroupPermissionBranchesDto
                {
                    BranchId = x.GroupPermissionBranches.FirstOrDefault().StoreBranchId,
                    GroupPermissionIds = x.GroupPermissionBranches.Select(x => x.GroupPermissionId).ToList(),
                }).ToList();

            // The data after processing.
            staff.PermissionGroupControls = GetPermissionGroupControls(groupsBeforeHandling).ToList();

            return new GetStaffByIdResponse
            {
                Staff = staff
            };
        }

        /// <summary>
        /// This method is used to group the permissions and branches.
        /// </summary>
        /// <param name="groupsBeforeHandling"></param>
        /// <returns></returns>
        private IEnumerable<StaffByIdModel.GroupPermissionBranchesDto> GetPermissionGroupControls(
            List<StaffByIdModel.GroupPermissionBranchesDto> groupsBeforeHandling
        )
        {

            // Merge all branches with the same permission group.
            var groupsAfterHandling = groupsBeforeHandling.
                    GroupBy(x =>
                        string.Join(";", x.GroupPermissionIds.
                        OrderBy(gp => gp).
                        Select(gp => gp.
                        ToString())
                    )
                ).ToList();

            // For example:
            /**
             - group 1: {key: "branch 1;branch 2", permissions: ["Admin", "Mod", "User"] }
             - group 2: {key: "branch 3", permissions: ["Super Admin"] }
             */
            foreach (IGrouping<string, StaffByIdModel.GroupPermissionBranchesDto> item in groupsAfterHandling)
            {
                // Brand id list, for example: ["d55b5a76-8592-4a89-9987-cb03e4ac9875", "830f178f-9154-44d8-aa6e-69d792c6b017"]
                var branchList = item.
                    Select(gpb => gpb.BranchId).
                    Where(branchId => branchId.HasValue).
                    Distinct().
                    Select(branchId => branchId.Value).
                    ToList();

                // For example:
                /**
                 - item 1: {branchId: "d55b5a76-8592-4a89-9987-cb03e4ac9875", permissions: ["eab70c13-0a6a-45a4-aeb0-c0d426a5339f", "7dd680da-5acd-4c73-9db2-d6d6be462408"]}
                 - item 2: {branchId: "830f178f-9154-44d8-aa6e-69d792c6b017", permissions: ["eab70c13-0a6a-45a4-aeb0-c0d426a5339f", "7dd680da-5acd-4c73-9db2-d6d6be462408"]}
                 */
                // You can see in this example, they have different branches but the same permissions.
                var groups = item.FirstOrDefault().GroupPermissionIds;

                // Create an object to save this value to display on the page.
                var aGroupItem = new StaffByIdModel.GroupPermissionBranchesDto();

                // Set the value and push them to the IEnumerable.
                aGroupItem.BranchIds = branchList;
                aGroupItem.GroupPermissionIds = groups;
                yield return aGroupItem;
            }

        }
    }
}
