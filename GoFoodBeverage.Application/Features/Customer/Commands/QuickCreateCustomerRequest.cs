using System;
using MediatR;
using AutoMapper;
using PhoneNumbers;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using GoFoodBeverage.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Customer;
using GoFoodBeverage.Common.Models.User;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class QuickCreateCustomerRequest : IRequest<QuickCreateCustomerResponse>
    {
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public string CountryCode { get; set; }

        public Guid? CountryId { get; set; }
    }

    public class QuickCreateCustomerResponse
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

    public class QuickCreateCustomerHandler : IRequestHandler<QuickCreateCustomerRequest, QuickCreateCustomerResponse>
    {
        private readonly IMapper _mapper;

        private readonly IJWTService _jwtService;

        private readonly IUnitOfWork _unitOfWork;


        public QuickCreateCustomerHandler(
            IMapper mapper,
            IJWTService jwtService,
            IUnitOfWork unitOfWork
        )
        {
            _mapper = mapper;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// This method is used to handle data from the HTTP request.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<QuickCreateCustomerResponse> Handle(QuickCreateCustomerRequest request, CancellationToken cancellationToken)
        {
            var dataToResponse = new QuickCreateCustomerResponse();
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Check data.
                var isValid = Validation(request);
                if (!isValid)
                {
                    dataToResponse.IsSuccess = false;
                    dataToResponse.Message = "There are some invalid data.";
                }
                else
                {
                    // Convert the phone number object by the phone number string.
                    PhoneNumber phoneNumber = PhoneHelpers.GetPhoneNumber(request.PhoneNumber, request.CountryCode);

                    // Get the customer information from the database.
                    var account = await _unitOfWork.Accounts
                        .GetAccountByPhoneNumberAsync(request.PhoneNumber, $"{phoneNumber.NationalNumber}");

                    // Encrypt the customer password.
                    var passwordHash = (new PasswordHasher<Domain.Entities.Account>()).HashPassword(null, request.Password.Trim());

                    // Re-active for the customer.
                    if (account != null && account.IsActivated == false)
                    {
                        account.Password = passwordHash;
                        account.FullName = request.FullName;
                        account.IsActivated = true;
                        account.PlatformId = EnumPlatform.GoFnBApp.ToGuid();

                        // Update the customer.
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        AccountType accountType = await _unitOfWork.AccountTypes
                            .GetAll()
                            .FirstOrDefaultAsync(at => at.EnumValue == (int)EnumAccountType.User);

                        // Create a new entity object to insert to the database.
                        account = _mapper.Map<Domain.Entities.Account>(request);
                        account.AccountTypeId = accountType.Id;

                        // Set values for the object.
                        account.Password = passwordHash;
                        account.IsActivated = true;
                        account.NationalPhoneNumber = $"{phoneNumber.NationalNumber}";
                        account.CountryId = request.CountryId;
                        account.PlatformId = EnumPlatform.GoFnBApp.ToGuid();

                        // Insert to the database.
                        await _unitOfWork.Accounts.AddAsync(account);
                        account.AccountType = accountType;
                    }

                    string accessToken = CreateAccessTokenKeyString(request, account);

                    // If there are no errors, finish this transaction to save data.
                    await transaction.CommitAsync();

                    var userInfo = _mapper.Map<CustomersModel>(account);
                    userInfo.CountryId = request.CountryId;
                    userInfo.CountryCode = request.CountryCode;

                    // Set data for the response.
                    dataToResponse.IsSuccess = true;
                    dataToResponse.Message = "Account created successfully!";
                    dataToResponse.Data = new QuickCreateCustomerResponse.DataModel
                    {
                        Token = accessToken,
                        CustomerInfo = userInfo
                    };
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                dataToResponse.IsSuccess = false;
                dataToResponse.Message = e.Message;
            }

            return dataToResponse;
        }

        /// <summary>
        /// This method is used for valid data from the HTTP request.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <returns>True/False</returns>
        private bool Validation(QuickCreateCustomerRequest request)
        {
            if (request == null)
            {
                return false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.FullName) ||
                   string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                   string.IsNullOrWhiteSpace(request.Password)
                   )
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// This method is used to create a new token string.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <param name="account">The account has been created and saved from database.</param>
        /// <returns>The token string</returns>
        private string CreateAccessTokenKeyString(QuickCreateCustomerRequest request, Domain.Entities.Account account)
        {
            // Data to create a new token key string to access our system.
            var user = new LoggedUserModel();
            user.Id = account.Id;
            user.NeverExpires = true;
            user.AccountId = account.Id;
            user.FullName = account.FullName;
            user.CountryId = account.CountryId;
            user.CountryCode = request.CountryCode;
            user.PhoneNumber = account.PhoneNumber;
            user.AccountType = $"{account.AccountType.EnumValue}";

            // Generate an access token string.
            return _jwtService.GenerateAccessToken(user);
        }
    }
}
