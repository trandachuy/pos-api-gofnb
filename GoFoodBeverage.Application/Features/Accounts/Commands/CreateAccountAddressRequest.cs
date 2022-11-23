using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Features.Account.Commands
{
    public class CreateAccountAddressRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string AddressDetail { get; set; }

        public double Lng { get; set; }

        public double Lat { get; set; }

        public string Note { get; set; }

        public EnumCustomerAddressType CustomerAddressTypeId { get; set; }

        public int Possion { get; set; }

    }

    public class CreateCustomerAddressRequestHandler : IRequestHandler<CreateAccountAddressRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;


        public CreateCustomerAddressRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(CreateAccountAddressRequest request, CancellationToken cancellationToken)
        {
            var account = _userProvider.GetLoggedCustomer();
            var customerExist = await _unitOfWork.Accounts.GetAccountActivatedByIdAsync(account.Id.Value);
            if (customerExist == null)
            {
                return false;
            }

            var newCustomerAddress = new AccountAddress()
            {
                AccountId = account.Id.Value,
                Name = request.Name,
                Address = request.Address,
                AddressDetail = request.AddressDetail,
                Lng = request.Lng,
                Lat = request.Lat,
                Note = request.Note,
                CustomerAddressTypeId = request.CustomerAddressTypeId,
                Possion = request.Possion
            };

            await _unitOfWork.AccountAddresses.AddAsync(newCustomerAddress);

            return true;
        }
    }
}
