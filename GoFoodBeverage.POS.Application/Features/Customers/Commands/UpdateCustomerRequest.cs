using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.POS.Application.Features.Customer.Commands
{
    public class UpdateCustomerRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDay { get; set; }

        public bool IsMale { get; set; }

        public string Note { get; set; }

        public Guid? AddressId { get; set; }

        public AddressDto Address { get; set; }

        public class AddressDto
        {
            public Guid? CountryId { get; set; }

            public Guid? StateId { get; set; }

            public Guid? CityId { get; set; }

            public Guid? DistrictId { get; set; }

            public Guid? WardId { get; set; }

            public string Address1 { get; set; }

            public string Address2 { get; set; }

            public string CityTown { get; set; }

            public string PostalCode { get; set; }
        }
    }

    public class UpdateCustomerRequestHandler : IRequestHandler<UpdateCustomerRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateCustomerRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);
            var customer = await _unitOfWork.Customers.GetCustomerByIdInStore(request.Id, loggerUser.StoreId.Value);
            if (customer == null) return false;

            CheckUniqueAndValidationAsync(request, loggerUser.StoreId.Value, customer);

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                customer.FullName = request.FullName;
                customer.PhoneNumber = request.Phone;
                customer.Email = request.Email;
                customer.Birthday = request.BirthDay;
                customer.Note = request.Note;
                customer.Gender = request.IsMale;
                await _unitOfWork.Customers.UpdateAsync(customer);

                if (request?.AddressId != null)
                {
                    var address = await _unitOfWork.Addresses.GetAddressByIdAsync(request.AddressId.Value);
                    if (!string.IsNullOrEmpty(request?.Address?.Address1) || request?.Address?.CityId != null)
                    {
                        address.Address1 = request?.Address?.Address1;
                        address.CityId = request?.Address?.CityId;
                        address.WardId = request?.Address?.WardId;
                        address.DistrictId = request?.Address?.DistrictId;
                        address.Address2 = request?.Address?.Address2;
                        address.StateId = request?.Address?.StateId;
                        address.CityTown = request?.Address?.CityTown;
                        address.CountryId = request?.Address?.CountryId;
                        await _unitOfWork.Addresses.UpdateAsync(address);
                    }
                    else
                    {
                        customer.AddressId = null;
                        await _unitOfWork.Customers.UpdateAsync(customer);
                        await _unitOfWork.Addresses.RemoveAsync(address);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(request?.Address?.Address1) || request?.Address?.CityId != null)
                    {
                        var addressAdd = new Address()
                        {
                            Address1 = request?.Address?.Address1,
                            WardId = request?.Address?.WardId,
                            DistrictId = request?.Address?.DistrictId,
                            CityId = request?.Address?.CityId,
                            Address2 = request?.Address?.Address2,
                            StateId = request?.Address?.StateId,
                            CityTown = request?.Address?.CityTown,
                            CountryId = request?.Address?.CountryId,
                        };
                        await _unitOfWork.Addresses.AddAsync(addressAdd);
                        customer.AddressId = addressAdd.Id;
                        await _unitOfWork.Customers.UpdateAsync(customer);
                    }
                }

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }
        }

        private void CheckUniqueAndValidationAsync(UpdateCustomerRequest request, Guid storeId, Domain.Entities.Customer customer)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.FullName), new JObject()
            {
                { $"{nameof(request.FullName)}",   "Please enter full name"},
            });
            ThrowError.Against(string.IsNullOrEmpty(request.Phone), new JObject()
            {
                { $"{nameof(request.Phone)}",  "Please enter phone"},
            });

            if (customer.FullName != request.FullName.Trim())
            {
                var customerNameExisted = _unitOfWork.Customers.CheckCustomerByNameInStore(request.FullName.Trim(), storeId);
                ThrowError.Against(customerNameExisted == true, new JObject()
                  {
                      { $"{nameof(request.FullName)}",  "This name is already existed"}
                  });
            }

            if (customer.PhoneNumber != request.Phone.Trim())
            {
                var phoneExisted = _unitOfWork.Customers.CheckCustomerByPhoneInStore(request.Phone.Trim(), storeId);
                ThrowError.Against(phoneExisted == true, new JObject()
                  {
                      { $"{nameof(request.Phone)}",  "Phone number is existed"},
                  });
            }

            if (!string.IsNullOrWhiteSpace(request.Email) & (customer.Email != request?.Email?.Trim()))
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
