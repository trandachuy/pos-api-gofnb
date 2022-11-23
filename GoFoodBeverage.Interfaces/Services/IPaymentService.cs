using GoFoodBeverage.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces
{
    public interface IPaymentService
    {
        public Task<bool> PaymentRefundAsync(PaymentRefundRequestModel requestModel);
    }
}
