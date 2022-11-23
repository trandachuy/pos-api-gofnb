using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Settings.Commands
{
    public class UpdateGroupPermissionRequest : IRequest<bool>
    {
        public Guid GroupPermissionId { get; set; }

        public string GroupPermissionName { get; set; }

        public string Description { get; set; }

        public IEnumerable<Guid> PermissionIds { get; set; }
    }

    public class UpdateGroupPermissionRequestHandler : IRequestHandler<UpdateGroupPermissionRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public UpdateGroupPermissionRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateGroupPermissionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            var groupPermission = await _unitOfWork.GroupPermissions.GetGroupPermissionByIdInStoreAsync(request.GroupPermissionId, loggedUser.StoreId.Value);
            ThrowError.Against(groupPermission == null, "Cannot find group permission information");

            var groupPermissions = await _unitOfWork.GroupPermissions
                    .GetAll()
                    .Where(g => g.StoreId == loggedUser.StoreId && g.Id != request.GroupPermissionId)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);
            var groupPermissionNameExisted = groupPermissions.Find(i => i.Name == request.GroupPermissionName);
            ThrowError.Against(groupPermissionNameExisted != null, "Name of group permission has already existed");

            var staff = await _unitOfWork.Staffs.GetStaffByIdAsync(loggedUser.Id.Value);
            ThrowError.Against(staff == null, "Cannot find staff information, please authenticate before!");

            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId);
            ThrowError.Against(store == null, "Cannot find store information, please authenticate before!");

            var permissions = await _unitOfWork.Permissions
                .Find(p => request.PermissionIds.Any(id => id == p.Id))
                .ToListAsync(cancellationToken: cancellationToken);
            
            var modifiedGroupPermission = await UpdateGroupPermission(groupPermission, request, permissions, loggedUser.AccountId.Value);
            await _unitOfWork.GroupPermissions.UpdateAsync(modifiedGroupPermission);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(UpdateGroupPermissionRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.GroupPermissionName), "Please enter name of group permission.");
            ThrowError.Against(request.PermissionIds.Count() == 0, "Please select at least 1 permission.");
        }

        public async Task<GroupPermission> UpdateGroupPermission(GroupPermission groupPermission, UpdateGroupPermissionRequest request, List<Permission> permissions, Guid accountId)
        {
            groupPermission.Name = request.GroupPermissionName;
            groupPermission.Description = request.Description;
            groupPermission.LastSavedUser = accountId;
            groupPermission.LastSavedTime = DateTime.UtcNow;

            var currentGroupPermissionPermissions = groupPermission.GroupPermissionPermissions.ToList();
            var newGroupPermissionPermissions = new List<GroupPermissionPermission>();
            var listDeleteGroupPermissionPermissions = currentGroupPermissionPermissions.Where(p => !permissions.Select(i => i.Id).Contains(p.PermissionId));
            permissions.ForEach(permission =>
            {
                var permissionItem = currentGroupPermissionPermissions.FirstOrDefault(p => p.PermissionId == permission.Id);
                if (permissionItem == null)
                {
                    var groupPermissionPermission = new GroupPermissionPermission()
                    {
                        PermissionId = permission.Id,
                        GroupPermissionId = groupPermission.Id
                    };
                    newGroupPermissionPermissions.Add(groupPermissionPermission);
                }
            });

            await _unitOfWork.GroupPermissionPermissions.AddRangeAsync(newGroupPermissionPermissions);
            await _unitOfWork.GroupPermissionPermissions.RemoveRangeAsync(listDeleteGroupPermissionPermissions);

            return groupPermission;
        }
    }
}
