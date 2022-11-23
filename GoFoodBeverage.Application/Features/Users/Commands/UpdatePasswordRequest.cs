using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Users.Commands
{
    public class UpdatePasswordRequest : IRequest<bool>
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class UpdatePasswordHandler : IRequestHandler<UpdatePasswordRequest, bool>
    {
        private IUnitOfWork _unitOfWork;
        private IUserActivityService _userActivityService;
        private IUserService _userService;

        public UpdatePasswordHandler(IUserService userService, IUnitOfWork unitOfWork, IUserActivityService userActivityService)
        {
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
            _userService = userService;
        }

        public async Task<bool> Handle(UpdatePasswordRequest request, CancellationToken cancellationToken)
        {
            ThrowError.ArgumentIsNull(request, request.CurrentPassword);
            ThrowError.ArgumentIsNull(request, request.NewPassword);
            ThrowError.ArgumentIsNull(request, request.ConfirmPassword);
            ThrowError.Against(request.NewPassword != request.ConfirmPassword, new JObject()
                {
                    { $"{nameof(request.NewPassword)} && {nameof(request.ConfirmPassword)}", "The new password and confirmation password does not match" },
                });

            var passwordVerificationResult = _userService.PasswordValidation(request.CurrentPassword, out Domain.Entities.Account account);
            ThrowError.Against(!passwordVerificationResult, new JObject()
                {
                     { $"{nameof(request.CurrentPassword)}", "Password invalid" },
                });

            var passwordHash = (new PasswordHasher<Domain.Entities.Account>()).HashPassword(null, request.NewPassword);
            account.Password = passwordHash;

            _unitOfWork.Accounts.Update(account);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }
    }
}
