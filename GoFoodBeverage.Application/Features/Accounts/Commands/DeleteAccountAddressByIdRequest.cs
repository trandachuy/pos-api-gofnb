using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;


namespace GoFoodBeverage.Application.Features.Account.Commands
{
    public class DeleteAccountAddressByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCustomerAddressByIdRequestHandler : IRequestHandler<DeleteAccountAddressByIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeleteCustomerAddressByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeleteAccountAddressByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = _userProvider.GetLoggedCustomer();

            var accountAddress = await _unitOfWork.AccountAddresses.Find(m => m.Id == request.Id && m.AccountId == loggedUser.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(accountAddress == null, "Account Address is not found");

            await _unitOfWork.AccountAddresses.RemoveAsync(accountAddress);

            return true;
        }
    }

}
