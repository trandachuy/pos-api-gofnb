using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomerSegmentModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Member { get; set; }

        public int No { get; set; }

        public IEnumerable<CustomerDataBySegmentModel> Customers { get; set; }
    }
}
