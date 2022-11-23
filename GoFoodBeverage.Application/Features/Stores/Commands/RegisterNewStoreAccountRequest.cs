using System;
using MediatR;
using System.Resources;
using System.Threading;
using System.Reflection;
using GoFoodBeverage.Email;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Settings;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Providers.Email;
using GoFoodBeverage.Common.Helpers;

namespace GoFoodBeverage.Application.Features.Stores.Commands
{
    public class RegisterNewStoreAccountRequest : IRequest<bool>
    {
        public string StoreName { get; set; }

        public string FullName { get; set; }

        public Guid? CountryId { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public Guid? CurrencyId { get; set; }

        public Guid? BusinessAreaId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public Guid? StateId { get; set; }

        public Guid? CityId { get; set; }

        public Guid? DistrictId { get; set; }

        public Guid? WardId { get; set; }

        public string CityTown { get; set; }

        public string PostalCode { get; set; }

        public string LangCode { get; set; }
    }

    public class RegisterNewStoreAccountRequestHandler : IRequestHandler<RegisterNewStoreAccountRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSenderProvider _emailProvider;
        private readonly DomainFE _domainFE;
        private readonly IUserActivityService _userActivityService;
        private readonly IDateTimeService _dateTimeService;

        public RegisterNewStoreAccountRequestHandler(
            IUnitOfWork unitOfWork,
            IEmailSenderProvider emailProvider,
            IOptions<DomainFE> domainFE,
            IUserActivityService userActivityService,
            IDateTimeService dateTimeService
        )
        {
            _unitOfWork = unitOfWork;
            _emailProvider = emailProvider;
            _domainFE = domainFE.Value;
            _userActivityService = userActivityService;
            _dateTimeService = dateTimeService;
        }

        public async Task<bool> Handle(RegisterNewStoreAccountRequest request, CancellationToken cancellationToken)
        {
            RequestValidation(request);

            var country = await _unitOfWork.Countries.GetCountryByIdAsync(request.CountryId.Value);
            var isDefaultCountry = country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO;
            using var createNewStoreTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var password = StringHelpers.GeneratePassword();
                var staffAccount = await CreateStaffAccountAsync(request, password, cancellationToken);
                var newAddress = CreateStoreAddress(request, isDefaultCountry);
                var newStore = new Store()
                {
                    CurrencyId = request.CurrencyId.Value,
                    Address = newAddress,
                    BusinessAreaId = request.BusinessAreaId.Value,
                    Title = request.StoreName,
                    IsStoreHasKitchen = true,
                    CreatedUser = staffAccount.Id,
                    LastSavedUser = staffAccount.Id,
                    LastSavedTime = _dateTimeService.NowUtc
                };

                var newStaff = CreateStaff(request, newStore, staffAccount, cancellationToken);
                newStore.InitialStoreAccountId = staffAccount.Id;
                await _unitOfWork.Staffs.AddAsync(newStaff);
                await _unitOfWork.SaveChangesAsync();
                await SendEmailPasswordAsync(request.FullName, staffAccount.Username, password, request.LangCode);
                await createNewStoreTransaction.CommitAsync(cancellationToken);
                await _userActivityService.LogAsync(request);
            }
            catch (Exception)
            {
                await createNewStoreTransaction.RollbackAsync(cancellationToken);
                throw;
            }

            return true;
        }

        private static void RequestValidation(RegisterNewStoreAccountRequest request)
        {
            ThrowError.BadRequestAgainst(string.IsNullOrEmpty(request.StoreName), "Please enter StoreName");
            ThrowError.BadRequestAgainst(string.IsNullOrEmpty(request.FullName), "Please enter FullName");
            ThrowError.BadRequestAgainstNull(request.CurrencyId);
            ThrowError.BadRequestAgainstNull(request.BusinessAreaId);
            ThrowError.BadRequestAgainstNull(request.CountryId);
        }

        private async Task<bool> SendEmailPasswordAsync(string fullName, string emailAddress, string password, string langCode)
        {
            ResourceManager myManager = new("GoFoodBeverage.Application.Providers.Email.EmailTemplate", Assembly.GetExecutingAssembly());
            var link = $"{_domainFE.EndUser}/login?username={emailAddress?.Trim()}";
            string templateMail = langCode switch
            {
                LanguageCodeConstants.VI => EmailTemplates.REGISTER_NEW_STORE_ACCOUNT_VI,
                _ => EmailTemplates.REGISTER_NEW_STORE_ACCOUNT,
            };

            string htmlContext = string.Format(myManager.GetString(templateMail),
                                $"{DefaultConstants.SYSTEM_NAME}",
                                $"{fullName}",
                                $"{emailAddress}",
                                $"{password}",
                                link);

            return await _emailProvider.SendEmailAsync(DefaultConstants.MAIL_SUBJECT, htmlContext, emailAddress);
        }

        private async Task<Domain.Entities.Account> CreateStaffAccountAsync(RegisterNewStoreAccountRequest request, string password, CancellationToken cancellationToken)
        {
            var passwordHash = (new PasswordHasher<Domain.Entities.Account>()).HashPassword(null, password);
            var validateCode = StringHelpers.GenerateValidateCode();
            var staffAccountType = await _unitOfWork.AccountTypes
                .Find(s => s.EnumValue == (int)EnumAccountType.Staff)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(staffAccountType == null, "Cannot find staff type account");

            var staffAccount = new Domain.Entities.Account()
            {
                Username = request.Email,
                Password = passwordHash,
                EmailConfirmed = true, /// bypass email confirm, will be remove in the feature
                ValidateCode = validateCode,
                AccountTypeId = staffAccountType.Id
            };

            return staffAccount;
        }

        private static Address CreateStoreAddress(RegisterNewStoreAccountRequest request, bool isDefaultCountry)
        {
            var address = new Address()
            {
                CountryId = request.CountryId,
                StateId = request.StateId,// If the country another default (VN), value is NULL
                CityTown = request.CityTown,// If the country another default (VN), value is NULL
                CityId = request.CityId,
                Address1 = request.Address1,
                Address2 = request.Address2,
                PostalCode = request.PostalCode,
            };
            if (isDefaultCountry)
            {
                address.DistrictId = request.DistrictId;
                address.WardId = request.WardId;
            }

            return address;
        }

        private static Staff CreateStaff(RegisterNewStoreAccountRequest request, Store store, Domain.Entities.Account staffAccount, CancellationToken cancellationToken)
        {
            var staff = new Staff()
            {
                Account = staffAccount,
                Store = store,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };

            return staff;
        }
    }
}
