using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public async Task<Address> GetAddressByIdAsync(Guid id)
        {
            var address = await dbSet.FirstOrDefaultAsync(c => c.Id == id);
            return address;
        }
    }
}
