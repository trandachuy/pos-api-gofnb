using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomerDataBySegmentModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public int No { get; set; }

        public string Rank { get; set; }

        public decimal? Point { get; set; }
    }
}
