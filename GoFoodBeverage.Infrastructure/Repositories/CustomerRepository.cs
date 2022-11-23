using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Customer> GetCustomerByKeySearchInStore(string keySearch, Guid? storeId)
        {
            IQueryable<Customer> customers;
            if (keySearch == null)
            {
                customers = dbSet.Where(c => c.StoreId == storeId);
            }
            else
            {
                keySearch = keySearch.Trim();
                customers = dbSet.Where(c => c.StoreId == storeId && (c.FullName.ToLower().Contains(keySearch.ToLower()) || c.PhoneNumber.ToLower().Contains(keySearch.ToLower())));
            }

            return customers;
        }

        public bool CheckCustomerByNameInStore(string name, Guid? storeId)
        {
            var customer = dbSet.FirstOrDefault(c => c.StoreId == storeId && c.FullName.ToLower() == name.ToLower());

            return customer == null ? false : true;
        }

        public bool CheckCustomerByPhoneInStore(string phone, Guid? storeId)
        {
            var customer = dbSet.FirstOrDefault(c => c.StoreId == storeId && c.PhoneNumber == phone);

            return customer != null;
        }

        public bool CheckCustomerByEmailInStore(string email, Guid? storeId)
        {
            var customer = dbSet.FirstOrDefault(c => c.StoreId == storeId && c.Email == email);

            return customer != null;
        }

        public async Task<Customer> GetCustomerByIdInStore(Guid id, Guid? storeId)
        {
            var customer = await dbSet.FirstOrDefaultAsync(c => c.StoreId == storeId && c.Id == id);

            return customer;
        }

        public IQueryable<Customer> GetAllCustomersInStore(Guid storeId)
        {
            var customers = dbSet.Where(s => s.StoreId == storeId);

            return customers;
        }

        public IQueryable<Customer> GetCustomersbyAccumulatedPointInStore(int accumulatedPoint, Guid? storeId)
        {
            var listCustomerIds = _dbContext.CustomerPoints.Where(x => x.AccumulatedPoint >= accumulatedPoint).Select(x => x.CustomerId).ToList();
            var customers = _dbContext.Customers.Where(x => listCustomerIds.Contains(x.Id) && x.StoreId == storeId);

            return customers;
        }

        public List<Customer> GetCustomerDataBySegmentInStore(Guid storeId, Guid segmentId)
        {
            string sql = "EXEC dbo.SP_CalculateCustomersBySegment @StoreId, @CustomerSegmentId";
            List<SqlParameter> parms = new()
            {
                /// Create parameter(s)    
                new SqlParameter { ParameterName = "@StoreId", Value = storeId },
                new SqlParameter { ParameterName = "@CustomerSegmentId", Value = segmentId },
            };

            List<Customer> customers = _dbContext.Customers.FromSqlRaw(sql, parms.ToArray()).ToList();

            return customers;
        }

        public async Task<Customer> GetCustomerByIdAsync(Guid id)
        {
            Customer customer = await dbSet.Where(a => a.Id == id)
                .Include(a => a.Store)
                .Include(a => a.Address)
                // .Include(a => a.Country)
                .FirstOrDefaultAsync();

            return customer;
        }
    }
}