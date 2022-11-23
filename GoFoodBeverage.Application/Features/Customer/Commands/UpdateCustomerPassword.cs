using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Threading;
using MediatR;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class UpdateCustomerPassword : IRequest<UpdateCustomerPasswordResponse>
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
    public class UpdateCustomerPasswordResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
        public string ObjectName { get; set; }
    }


    public class UpdateCustomerPasswordHandler : IRequestHandler<UpdateCustomerPassword, UpdateCustomerPasswordResponse>
    {
        private IUnitOfWork _unitOfWork;
        private IUserActivityService _userActivityService;
        private IUserService _userService;

        public UpdateCustomerPasswordHandler(IUserService userService, IUnitOfWork unitOfWork, IUserActivityService userActivityService)
        {
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
            _userService = userService;
        }

        public async Task<UpdateCustomerPasswordResponse> Handle(UpdateCustomerPassword request, CancellationToken cancellationToken)
        {
            ThrowError.ArgumentIsNull(request, request.CurrentPassword);
            ThrowError.ArgumentIsNull(request, request.NewPassword);
            ThrowError.ArgumentIsNull(request, request.ConfirmPassword);
            ThrowError.Against(request.NewPassword != request.ConfirmPassword, new JObject()
                {
                    { $"{nameof(request.NewPassword)} && {nameof(request.ConfirmPassword)}", "The new password and confirmation password does not match" },
                });

            var passwordVerificationResult = _userService.GoAppPasswordValidation(request.CurrentPassword, out Domain.Entities.Account account);
            if (!passwordVerificationResult)
            {
                return new UpdateCustomerPasswordResponse
                {
                    IsSuccess = false,
                    Message = "message.passwordIsIncorrect",
                    ObjectName = "currentPassword"
                };
            }

            var passwordHash = new PasswordHasher<Domain.Entities.Account>().HashPassword(null, request.NewPassword);
            account.Password = passwordHash;

            _unitOfWork.Accounts.Update(account);
            await _unitOfWork.SaveChangesAsync();
            await _userActivityService.LogAsync(request);

            return new UpdateCustomerPasswordResponse
            {
                IsSuccess = true,
                Message = "message.updateComplete"
            };
        }
    }
}
