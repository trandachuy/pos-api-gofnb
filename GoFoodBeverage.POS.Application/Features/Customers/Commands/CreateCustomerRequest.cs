using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.POS.Application.Features.Customer.Commands
{
    public class CreateCustomerRequest : IRequest<bool>
    {
        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime? BirthDay { get; set; }

        public bool IsMale { get; set; }

        public string Note { get; set; }

        public Guid? AddressId { get; set; }

        public AddressDto Address { get; set; }

        public class AddressDto
        {
            public Guid? CountryId { get; set; }

            public Guid? StateId { get; set; } // State

            public Guid? CityId { get; set; } // Province / city / town

            public Guid? DistrictId { get; set; } // District

            public Guid? WardId { get; set; } // ward

            public string Address1 { get; set; }

            public string Address2 { get; set; }

            public string CityTown { get; set; }

            public string PostalCode { get; set; } // zip / postal code
        }
    }

    public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateCustomerHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);

            CheckUniqueAndValidation(request, loggerUser.StoreId.Value);

            var newCustomer = new Domain.Entities.Customer()
            {
                FullName = request.FullName.Trim(),
                PhoneNumber = request.Phone.Trim(),
                Email = request.Email?.Trim(),
                Birthday = request.BirthDay,
                Gender = request.IsMale,
                Note = request.Note?.Trim(),
                StoreId = loggerUser.StoreId.Value,
                CreatedUser = loggerUser.AccountId.Value,
                PlatformId = EnumPlatform.POSWebsite.ToGuid(),
                BranchId = loggerUser.BranchId.Value
            };

            if (request?.Address?.CountryId != null)
            {
                newCustomer.Address = new Address()
                {
                    CountryId = request.Address.CountryId,
                    Address1 = request?.Address?.Address1,
                    Address2 = request?.Address?.Address2,
                    CityTown = request?.Address?.CityTown,
                    StateId = request?.Address?.StateId,
                    WardId = request?.Address?.WardId,
                    DistrictId = request?.Address?.DistrictId,
                    CityId = request?.Address?.CityId,
                    PostalCode = request?.Address?.PostalCode,
                };

                newCustomer.AddressId = newCustomer.Address.Id;
            }

            var customerPoint = new CustomerPoint()
            {
                CustomerId = newCustomer.Id,
                AccumulatedPoint = 0
            };

            newCustomer.CustomerPoint = customerPoint;

            await _unitOfWork.Customers.AddAsync(newCustomer);

            return true;
        }

        private void CheckUniqueAndValidation(CreateCustomerRequest request, Guid storeId)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.FullName), new JObject()
            {
                { $"{nameof(request.FullName)}",   "Please enter fullName name"},
            });

            ThrowError.Against(string.IsNullOrEmpty(request.Phone), new JObject()
            {
                { $"{nameof(request.Phone)}",  "Please enter phone"},
            });

            var customerNameExisted = _unitOfWork.Customers.CheckCustomerByNameInStore(request.FullName, storeId);
            ThrowError.Against(customerNameExisted == true, new JObject()
            {
                { $"{nameof(request.FullName)}",  "This name is already existed"},
            });

            var phoneExisted = _unitOfWork.Customers.CheckCustomerByPhoneInStore(request.Phone.Trim(), storeId);
            ThrowError.Against(phoneExisted == true, new JObject()
            {
                { $"{nameof(request.Phone)}",  "Phone number is existed"},
            });

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExisted = _unitOfWork.Customers.CheckCustomerByEmailInStore(request.Email.Trim(), storeId);
                ThrowError.Against(emailExisted == true, new JObject()
                {
                    { $"{nameof(request.Email)}",  "Email is existed"},
                });
            }
        }
    }
}
