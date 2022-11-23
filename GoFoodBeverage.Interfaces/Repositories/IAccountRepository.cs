using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        /// <summary>
        /// Get all accounts in system
        /// </summary>
        /// <returns></returns>
        Task<List<Account>> GetAccountsAsync();

        /// <summary>
        /// Find account information by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<Account> GetAccountByUsernameAsync(string username);

        /// <summary>
        /// Find account information by accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Account> GetIdentifierAsync(Guid userId);

        /// <summary>
        /// Find account information by accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Account GetIdentifier(Guid accountId);


        /// <summary>
        /// This method is used to get the account by the phone number;
        /// </summary>
        /// <param name="phoneNumber">Account's phone number, for example: 0909123456.</param>
        /// <param name="nationalNumber">The national number of the phone number, for example: before converting 0909123456 -> after converting 909123456</param>
        /// <returns></returns>
        Task<Account> GetAccountByPhoneNumberAsync(string phoneNumber, string nationalNumber);

        /// <summary>
        /// This method is used to get the customer by the login information;
        /// </summary>
        /// <param name="loginInfo">This is the customer's phone number or email address.</param>
        /// <returns>Customer</returns>
        Task<Account> GetAccountAsync(string loginInfo);

        /// <summary>
        /// This method is used to get the customer status by the customer id.
        /// </summary>
        /// <param name="id">The customer id, for example: fa4dddae-ac71-4987-bf82-54224c1fcce5</param>
        /// <returns>EnumCustomerStatus</returns>
        Task<bool> GetAccountStatusByIdAsync(Guid id);

        Task<Account> GetAccountActivatedByIdAsync(Guid id);

    }
}
