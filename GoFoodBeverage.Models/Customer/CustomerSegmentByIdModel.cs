using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomerSegmentByIdModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public bool IsAllMatch { get; set; }

        public List<CustomerSegmentConditionDataModel> CustomerSegmentConditions { get; set; }
    }
}
