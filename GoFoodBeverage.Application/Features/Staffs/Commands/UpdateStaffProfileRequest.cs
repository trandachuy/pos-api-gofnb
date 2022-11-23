using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Features.Staffs.Commands
{

    public class UpdateStaffProfileRequest : IRequest<UpdateStaffProfileResponse>
    {
        public Guid UserId { get; set; }

        public string Fullname { get; set; }

        public string PhoneNumber { get; set; }

        public string Thumbnail { get; set; }
    }

    public class UpdateStaffProfileResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }

    public class UpdateStaffProfileRequestHandler : IRequestHandler<UpdateStaffProfileRequest, UpdateStaffProfileResponse>
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        public UpdateStaffProfileRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<UpdateStaffProfileResponse> Handle(UpdateStaffProfileRequest request, CancellationToken cancellationToken)
        {
            var response = new UpdateStaffProfileResponse();
            // Get the current user information from the user token.
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            Staff staffEntity = await _unitOfWork.Staffs.GetStaffByIdAsync(loggedUser.Id ?? Guid.Empty);
            if (staffEntity == null)
            {
                response.IsSuccess = false;
                response.Message = "myAccount.notExist";
                return response;
            }

            // Staff phone unique inside tenant
            var staffPhoneExisted = await _unitOfWork.Staffs
                .GetAllStaffInStore(loggedUser.StoreId ?? Guid.Empty)
                .AsNoTracking()
                .AnyAsync(s => s.Id != staffEntity.Id && s.PhoneNumber == request.PhoneNumber);
            if (staffPhoneExisted)
            {
                response.IsSuccess = false;
                response.Message = "myAccount.tabName.phoneNumberExistedMessage";
                return response;
            }

            staffEntity.FullName = request.Fullname;
            staffEntity.PhoneNumber = request.PhoneNumber;
            staffEntity.Thumbnail = request.Thumbnail;
            await _unitOfWork.Staffs.UpdateAsync(staffEntity);
            response.IsSuccess = true;

            return response;
        }
    }
}
