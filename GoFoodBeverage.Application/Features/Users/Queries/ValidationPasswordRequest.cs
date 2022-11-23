using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Users.Queries
{
    public class ValidationPasswordRequest : IRequest<bool>
    {
        public string CurrentPassword { get; set; }
    }

    public class ValidationPassword : IRequestHandler<ValidationPasswordRequest, bool>
    {
        private readonly IUserService _userService;

        public ValidationPassword(IUserService userService)
        {
            _userService = userService;
        }

        public Task<bool> Handle(ValidationPasswordRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Account _;
            var passwordVerificationResult = _userService.PasswordValidation(request.CurrentPassword, out _);

            return Task.FromResult(passwordVerificationResult);
        }
    }
}
