using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Application.Features.Account.Commands
{
    public class UpdateAccountAddressByIdRequest : IRequest<UpdateCustomerAddressByIdResponse>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string AddressDetail { get; set; }

        public double Lng { get; set; }

        public double Lat { get; set; }

        public string Note { get; set; }
    }

    public class UpdateCustomerAddressByIdResponse
    {
        public bool IsSuccess { get; set; }
    }

    public class UpdateCustomerAddressByIdRequestHanlder : IRequestHandler<UpdateAccountAddressByIdRequest, UpdateCustomerAddressByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdateCustomerAddressByIdRequestHanlder(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<UpdateCustomerAddressByIdResponse> Handle(UpdateAccountAddressByIdRequest request, CancellationToken cancellationToken)
        {
            var response = new UpdateCustomerAddressByIdResponse();
            // Get the current user information from the user token.
            var loggedUser = _userProvider.GetLoggedCustomer();
            Guid? customerId = loggedUser.Id;

            if (!customerId.HasValue)
            {
                response.IsSuccess = false;
                return response;
            }

            var accountAddress = await _unitOfWork.AccountAddresses.Find(a => a.Id == request.Id && a.AccountId == loggedUser.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (accountAddress == null)
            {
                response.IsSuccess = false;
                return response;
            }

            accountAddress.Name = request.Name;
            accountAddress.Address = request.Address;
            accountAddress.AddressDetail = request.AddressDetail;
            accountAddress.Lat = request.Lat;
            accountAddress.Lng = request.Lng;
            accountAddress.Note = request.Note;

            await _unitOfWork.AccountAddresses.UpdateAsync(accountAddress);

            response.IsSuccess = true;

            return response;
        }
    }
}
