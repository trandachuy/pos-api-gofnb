using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using GoFoodBeverage.Interfaces.Repositories;
using System.Linq;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public async Task<List<Account>> GetAccountsAsync()
        {
            var accounts = await dbSet.Where(u => !u.IsDeleted).ToListAsync();
            return accounts;
        }

        public async Task<Account> GetIdentifierAsync(Guid accountId)
        {
            var account = await dbSet.FirstOrDefaultAsync(u => u.Id == accountId && !u.IsDeleted);

            return account;
        }

        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            var account = await dbSet.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);

            return account;
        }

        public Account GetIdentifier(Guid accountId)
        {
            var account = dbSet.AsNoTracking().FirstOrDefault(u => u.Id == accountId && !u.IsDeleted);

            return account;
        }


        /// <summary>
        /// This method is used to get the customer by the phone number;
        /// </summary>
        /// <param name="phoneNumber">Customer's phone number, for example: 0909123456.</param>
        /// <param name="nationalNumber">The national number of the phone number, for example: before converting 0909123456 -> after converting 909123456</param>
        /// <returns>Customer</returns>
        public async Task<Account> GetAccountByPhoneNumberAsync(string phoneNumber, string nationalNumber)
        {

            Account account = await _dbContext.
                Accounts.
                SingleOrDefaultAsync(customer =>
                    customer.PhoneNumber == phoneNumber ||
                    customer.NationalPhoneNumber == nationalNumber
                );

            return account;

        }


        /// <summary>
        /// This method is used to get the customer by the login information;
        /// </summary>
        /// <param name="loginInfo">This is the customer's phone number or email address.</param>
        /// <returns>Customer</returns>
        public async Task<Account> GetAccountAsync(string loginInfo)
        {
            Account account = await _dbContext
                .Accounts
                .Include(cus => cus.Country)
                .Include(cus => cus.AccountType)
                .FirstOrDefaultAsync(cus =>
                    cus.PhoneNumber == loginInfo ||
                    cus.Username == loginInfo ||
                    //loginInfo.EndsWith(cus.PhoneNumber)
                    cus.NationalPhoneNumber == loginInfo
                );

            return account;
        }

        /// <summary>
        /// This method is used to get the customer status by the customer id.
        /// </summary>
        /// <param name="id">The customer id, for example: fa4dddae-ac71-4987-bf82-54224c1fcce5</param>
        /// <returns>EnumCustomerStatus</returns>
        public async Task<bool> GetAccountStatusByIdAsync(Guid id)
        {
            bool status = await dbSet.
                Where(cus => cus.Id == id).
                Select(cus => cus.IsActivated).
                SingleOrDefaultAsync();

            return status;
        }

        public async Task<Account> GetAccountActivatedByIdAsync(Guid id)
        {
            var account = await dbSet.FirstOrDefaultAsync(a => a.Id == id && a.IsActivated && !a.IsDeleted);

            return account;
        }
    }
}
