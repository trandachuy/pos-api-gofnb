using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.DeliveryConfig
{
    public class AhaMoveConfigModel
    {
        public Guid DeliveryMethodId { get; set; }

        public string ApiKey { get; set; }

        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
    }
}
