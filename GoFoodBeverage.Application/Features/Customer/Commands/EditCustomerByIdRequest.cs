using System;
using MediatR;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class EditCustomerByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDay { get; set; }

        public bool IsMale { get; set; }

        public string Note { get; set; }

        public Guid? AddressId { get; set; }

        public string Thumbnail { get; set; }

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

    public class EditCustomerHandler : IRequestHandler<EditCustomerByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public EditCustomerHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(EditCustomerByIdRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);
            var customer = await _unitOfWork.Customers.GetCustomerByIdInStore(request.Id, loggerUser.StoreId.Value);
            if (customer == null) return false;
            CheckUniqueAndValidationAsync(request, loggerUser.StoreId.Value, customer);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
                try
                {
                    customer.FullName = request.FullName;
                    customer.PhoneNumber = request.Phone;
                    customer.Email = request.Email;
                    customer.Birthday = request.BirthDay;
                    customer.Note = request.Note;
                    customer.Gender = request.IsMale;
                    customer.Thumbnail = string.IsNullOrEmpty(request.Thumbnail) ? null : request.Thumbnail;
                    await _unitOfWork.Customers.UpdateAsync(customer);

                    if (request?.AddressId != null)
                    {
                        var address = await _unitOfWork.Addresses.GetAddressByIdAsync(request.AddressId.Value);
                        if (request?.Address?.CountryId != null)
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
                        if (request?.Address?.CountryId != null)
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

                    await transaction.CommitAsync();

                    await _mediator.Send(new CreateStaffActivitiesRequest()
                    {
                        ActionGroup = EnumActionGroup.Customer,
                        ActionType = EnumActionType.Edited,
                        ObjectId = customer.Id,
                        ObjectName = string.IsNullOrEmpty(customer.FullName) ? $"{customer.LastName} {customer.FirstName}" : customer.FullName,
                        ObjectThumbnail = customer.Thumbnail
                    });

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
        }

        private void CheckUniqueAndValidationAsync(EditCustomerByIdRequest request, Guid storeId, Domain.Entities.Customer customer)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.FullName), new JObject()
            {
                { $"{nameof(request.FullName)}",   "Please enter first name"},
            });
            ThrowError.Against(string.IsNullOrEmpty(request.Phone), new JObject()
            {
                { $"{nameof(request.Phone)}",  "Please enter phone"},
            });

            if (customer.FullName != request.FullName.Trim())
            {
                var customerNameExisted = _unitOfWork.Customers.CheckCustomerByNameInStore(request.FullName, storeId);
                ThrowError.Against(customerNameExisted == true, new JObject()
                  {
                      { $"{nameof(request.FullName)}",  "This name is already existed"},
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

            if (!string.IsNullOrWhiteSpace(customer.Email) && customer.Email != request.Email.Trim())
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
