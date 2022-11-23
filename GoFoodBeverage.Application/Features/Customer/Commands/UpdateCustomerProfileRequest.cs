using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Common;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Customer;
using System.Linq;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class UpdateCustomerProfileRequest : IRequest<UpdateCustomerProfileResponse>
    {
        public string Fullname { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public Guid CountryId { get; set; }
    }

    public class UpdateCustomerProfileResponse : ResponseCommonModel
    {
        public string ObjectName { get; set; }

        public CustomersModel CustomerInfo { get; set; }
    }

    public class UpdateCustomerProfileRequestHanlder : IRequestHandler<UpdateCustomerProfileRequest, UpdateCustomerProfileResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdateCustomerProfileRequestHanlder(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<UpdateCustomerProfileResponse> Handle(UpdateCustomerProfileRequest request, CancellationToken cancellationToken)
        {
            var response = new UpdateCustomerProfileResponse();
            // Get the current user information from the user token.
            var loggedUser = _userProvider.GetLoggedCustomer();
            Guid? customerId = loggedUser.Id;
            if (!customerId.HasValue)
            {
                response.IsSuccess = false;
                response.Message = "message.updateProfileNotLogin";
                return response;
            }

            var customerInformation = await _unitOfWork.Accounts
                .GetAll()
                .Include(a => a.Country)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(x => x.Id == customerId.Value);
            if (customerInformation == null)
            {
                response.IsSuccess = false;
                response.Message = "message.accountNotExist";
                return response;
            }

            if(!string.IsNullOrEmpty(request.Email))
            {
                bool emailExist = await _unitOfWork.
                Accounts.
                GetAll().
                AnyAsync(a => a.Id != customerId && a.Username == request.Email);
                if (emailExist)
                {
                    response.IsSuccess = false;
                    response.ObjectName = "email";
                    response.Message = "message.emailExisted";
                    return response;
                }
            }

            bool phoneNumberExist = await _unitOfWork.
                Accounts.
                GetAll().
                AnyAsync(a => a.Id != customerId && a.PhoneNumber == request.PhoneNumber);
            if (phoneNumberExist)
            {
                response.IsSuccess = false;
                response.ObjectName = "phoneNumber";
                response.Message = "message.phoneNumberExisted";
                return response;
            }

            customerInformation.Username = request.Email;
            customerInformation.PhoneNumber = request.PhoneNumber;
            customerInformation.FullName = request.Fullname;
            customerInformation.CountryId = request.CountryId;

            await _unitOfWork.Accounts.UpdateAsync(customerInformation);

            response.IsSuccess = true;
            response.Message = "message.updateComplete";
            response.CustomerInfo = _mapper.Map<CustomersModel>(customerInformation);
            response.CustomerInfo.CountryId = customerInformation.CountryId;

            var country = await _unitOfWork.Countries.GetCountryByIdAsync(customerInformation.CountryId.Value);
            response.CustomerInfo.CountryCode = country?.Iso;
            response.CustomerInfo.PhoneCode = country?.Phonecode;

            return response;
        }
    }
}
