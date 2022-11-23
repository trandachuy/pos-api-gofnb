using System;
using MediatR;
using PhoneNumbers;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Helpers;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class CheckAccountAlreadyExistsInSystemRequest : IRequest<CheckAccountAlreadyExistsInSystemResponse>
    {
        public string CountryCode { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class CheckAccountAlreadyExistsInSystemResponse
    {
        public bool IsSuccess { get; set; }

        public string PhoneNumberToSendOtpCode { get; set; }

        public string Message { get; set; }
    }

    public class CheckAccountAlreadyExistsInSystemRequestHandler : IRequestHandler<CheckAccountAlreadyExistsInSystemRequest, CheckAccountAlreadyExistsInSystemResponse>
    {
        private IUnitOfWork _unitOfWork;

        public CheckAccountAlreadyExistsInSystemRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// This method is used to handle data from the HTTP request.
        /// </summary>
        /// <param name="request">Data from the HTTP request.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CheckAccountAlreadyExistsInSystemResponse> Handle(CheckAccountAlreadyExistsInSystemRequest request, CancellationToken cancellationToken)
        {
            // Create a new object to return the logic method.
            var dataToResponse = new CheckAccountAlreadyExistsInSystemResponse();

            try
            {
                // Remove all spaces from the string.
                request.PhoneNumber = request.PhoneNumber?.Trim();

                // If data is not valid.
                if (string.IsNullOrEmpty(request.PhoneNumber))
                {
                    dataToResponse.Message = "There are some invalid data.";
                }

                PhoneNumber phoneNumber = PhoneHelpers.GetPhoneNumber(request.PhoneNumber, request.CountryCode);

                // Get the customer information from the database.
                var customer = await _unitOfWork.
                    Accounts.
                    GetAccountByPhoneNumberAsync(request.PhoneNumber, $"{phoneNumber.NationalNumber}");

                // If the user does not exist in the system, the user can register a new account.
                if (customer == null || !customer.IsActivated)
                {
                    dataToResponse.IsSuccess = true;
                    dataToResponse.PhoneNumberToSendOtpCode = $"+{phoneNumber.CountryCode}{phoneNumber.NationalNumber}";
                }
                else
                {
                    dataToResponse.Message = "This phone number is exited.";
                }
            }
            catch (Exception e)
            {
                dataToResponse.Message = e.Message;
            }

            // Return data.
            return dataToResponse;
        }
    }
}
