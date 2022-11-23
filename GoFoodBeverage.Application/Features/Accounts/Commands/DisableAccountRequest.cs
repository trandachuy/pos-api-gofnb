using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Account.Commands
{
    public class DisableAccountRequest : IRequest<bool>
    {
    }

    public class DisableAccountRequestHandler : IRequestHandler<DisableAccountRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public DisableAccountRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// This method is used to handle the current HTTP request.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns>Boolean</returns>
        public async Task<bool> Handle(DisableAccountRequest request, CancellationToken cancellationToken)
        {
            // Get account information from token.
            var loggedUser = _userProvider.GetLoggedCustomer();

            // Get the current account from the database by the account information (get from token).
            var account = await _unitOfWork.Accounts.GetAccountActivatedByIdAsync(loggedUser.Id.Value);
            account.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

}
