using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Users.Commands
{
    public class InternalToolAuthenticate
    {
        public class Request : IRequest<Response>
        {
            public string UserName { get; set; }

            public string Password { get; set; }
        }

        public class Response
        {
            public string Token { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IJWTService _jwtService;

            public Handler(IUnitOfWork unitOfWork, IJWTService jwtService)
            {
                _unitOfWork = unitOfWork;
                _jwtService = jwtService;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                ThrowError.ArgumentIsNull(request, request.UserName);
                ThrowError.ArgumentIsNull(request, request.Password);

                var hasher = new PasswordHasher<Domain.Entities.Account>();
                var internalAccount = await _unitOfWork.InternalAccounts
                    .Find(a => a.Username == request.UserName.Trim().ToLower())
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                var verified = hasher.VerifyHashedPassword(null, internalAccount.Password, request.Password);
                if (verified != PasswordVerificationResult.Success)
                {
                    ThrowError.Against(internalAccount == null, "User name or password invalid");
                }

                var user = new LoggedUserModel
                {
                    AccountId = internalAccount.Id,
                    UserName = internalAccount.Username,
                    Email = internalAccount.Username,
                    Password = internalAccount.Password
                };

                var accessToken = _jwtService.GenerateInternalToolAccessToken(user);
                var response = new Response() { Token = accessToken };

                return response;
            }
        }
    }
}
