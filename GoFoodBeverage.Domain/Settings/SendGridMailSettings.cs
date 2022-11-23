using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Domain.Settings
{
    public class SendGridMailSettings
    {
        public string Email { get; set; }

        public string ApiKey { get; set; }
    }
}
