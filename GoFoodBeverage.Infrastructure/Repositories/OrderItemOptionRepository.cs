using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderItemOptionRepository : GenericRepository<OrderItemOption>, IOrderItemOptionRepository
    {
        public OrderItemOptionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}
