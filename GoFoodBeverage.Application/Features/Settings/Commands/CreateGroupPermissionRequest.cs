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
    public class CreateGroupPermissionRequest : IRequest<bool>
    {
        public string GroupPermissionName { get; set; }

        public string Description { get; set; }

        public IEnumerable<Guid> PermissionIds { get; set; }
    }

    public class CreateGroupPermissionRequestHandler : IRequestHandler<CreateGroupPermissionRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreateGroupPermissionRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateGroupPermissionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            var groupPermissionNameExisted = await _unitOfWork.GroupPermissions
                .Find(g => g.StoreId == loggedUser.StoreId && g.Name == request.GroupPermissionName)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(groupPermissionNameExisted != null, "Name of group permission has already existed");

            var staff = await _unitOfWork.Staffs.GetStaffByIdAsync(loggedUser.Id.Value);
            ThrowError.Against(staff == null, "Cannot find staff information, please authenticate before!");

            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId);
            ThrowError.Against(store == null, "Cannot find store information, please authenticate before!");

            var permissions = await _unitOfWork.Permissions
                .Find(p => request.PermissionIds.Any(id => id == p.Id))
                .ToListAsync(cancellationToken: cancellationToken);
            
            var newGroupPermission = CreateGroupPermission(request, staff.Id, store.Id, permissions);
            await _unitOfWork.GroupPermissions.AddAsync(newGroupPermission);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(CreateGroupPermissionRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.GroupPermissionName), "Please enter name of group permission.");
            ThrowError.Against(request.PermissionIds.Count() == 0, "Please select at least 1 permission.");
        }

        private static GroupPermission CreateGroupPermission(CreateGroupPermissionRequest request, Guid staffId, Guid? storeId, List<Permission> permissions)
        {
            var newGroupPermission = new GroupPermission()
            {
                CreatedByStaffId = staffId,
                StoreId = storeId,
                Name = request.GroupPermissionName,
                Description = request.Description,
                GroupPermissionPermissions = new List<GroupPermissionPermission>()
            };

            permissions.ForEach(p =>
            {
                var groupPermissionPermission = new GroupPermissionPermission()
                {
                    PermissionId = p.Id,
                    GroupPermissionId = newGroupPermission.Id
                };
                newGroupPermission.GroupPermissionPermissions.Add(groupPermissionPermission);
            });

            return newGroupPermission;
        }
    }
}
