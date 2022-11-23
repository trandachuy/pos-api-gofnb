using System;
using MediatR;
using AutoMapper;
using PhoneNumbers;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Identity;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Models.Customer;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Users.Commands
{
    public class CustomerAuthenticationRequest : IRequest<CustomerAuthenticationResponse>
    {
        public string LoginInfo { get; set; }

        public string Password { get; set; }
    }

    public class CustomerAuthenticationResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public DataModel Data { get; set; }

        public class DataModel
        {
            public string Token { get; set; }

            public CustomersModel CustomerInfo { get; set; }
        }
    }

    public class CustomerAuthenticationRequestHandler : IRequestHandler<CustomerAuthenticationRequest, CustomerAuthenticationResponse>
    {
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IJWTService _jwtService;

        public CustomerAuthenticationRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IJWTService jwtService
        )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        /// <summary>
        /// This method is used to handle data from the HTTP request.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<CustomerAuthenticationResponse> Handle(CustomerAuthenticationRequest request, CancellationToken cancellationToken)
        {
            var dataToResponse = new CustomerAuthenticationResponse();
            try
            {
                bool isValid = Validation(request);
                if (!isValid)
                {
                    dataToResponse.Message = "message.thereAreSomeInvalidData";
                }
                else
                {
                    // Find the customer in the database.
                    var account = await _unitOfWork.Accounts.GetAccountAsync(ConvertLoginInfo(request.LoginInfo));
                    if (account == null || (account?.AccountType != null && account.AccountType.EnumValue == (int)EnumAccountType.Staff))
                    {
                        dataToResponse.Message = "message.yourCredentialIsIncorrect";
                    }
                    else
                    {
                        var hasher = new PasswordHasher<Domain.Entities.Account>();
                        var comparePassword = hasher.VerifyHashedPassword(null, account.Password, request.Password);

                        // If the user's password does not match or the user is inactive.
                        if (comparePassword == PasswordVerificationResult.Failed ||
                            account.IsActivated == false || account.IsDeleted == true)
                        {
                            dataToResponse.Message = "message.yourCredentialIsIncorrect";
                        }
                        else
                        {
                            var userInfo = _mapper.Map<CustomersModel>(account);
                            userInfo.CountryId = account.CountryId;
                            userInfo.CountryCode = account.Country?.Iso;

                            // Create a new token string by the customer information.
                            string accessToken = CreateAccessTokenKeyString(account);

                            // Set data for the response.
                            dataToResponse.IsSuccess = true;
                            dataToResponse.Message = "message.loggedInSuccessfully";
                            dataToResponse.Data = new CustomerAuthenticationResponse.DataModel
                            {
                                Token = accessToken,
                                CustomerInfo = userInfo
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                dataToResponse.Message = e.Message;
            }

            return dataToResponse;
        }

        /// <summary>
        /// This method is used to convert the login information.
        /// If the string is a phone number, we will receive a national phone number.
        /// If the converter throws an exception, it's probably the email address string.
        /// </summary>
        /// <param name="loginInfo">For example: abc@email.com or +841234567 or 01234567</param>
        /// <returns></returns>
        private static string ConvertLoginInfo(string loginInfo)
        {
            try
            {
                // Convert the phone number object by the phone number string.
                PhoneNumber phoneNumber = PhoneHelpers.GetPhoneNumber(loginInfo, null);
                return $"{phoneNumber.NationalNumber}";
            }
            catch
            {
                return loginInfo;
            }
        }

        /// <summary>
        /// This method is used for valid data from the HTTP request.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <returns>True/False</returns>
        private static bool Validation(CustomerAuthenticationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LoginInfo) || string.IsNullOrWhiteSpace(request.Password))
            {
                return false;
            }

            // Remove spaces.
            request.LoginInfo = request.LoginInfo.Trim().ToLower();
            request.Password = request.Password.Trim();

            return true;
        }

        /// <summary>
        /// This method is used to create a new token string.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <param name="account">The customer has been created and saved from database.</param>
        /// <returns>The token string</returns>
        private string CreateAccessTokenKeyString(Domain.Entities.Account account)
        {
            // Data to create a new token key string to access our system.
            var user = new LoggedUserModel
            {
                Id = account.Id,
                NeverExpires = true,
                AccountId = account.Id,
                FullName = account.FullName,
                CountryId = account.CountryId,
                PhoneNumber = account.PhoneNumber,
                CountryCode = account?.Country?.Iso,
                AccountType = $"{account.AccountType.EnumValue}",
                Email = account.Username
            };

            // Generate an access token string.
            return _jwtService.GenerateAccessToken(user);
        }
    }
}
