using System.Collections.Generic;

namespace GoFoodBeverage.Payment.MoMo.Model
{
    public class MomoErrorModel
    {
        public long ResponseTime { get; set; }

        public int ResultCode { get; set; }

        public string Message { get; set; }

        public List<MomoErrorItem> SubErrors { get; set; }

        public class MomoErrorItem
        {
            public string Field { get; set; }

            public string Message { get; set; }
        }
    }
}
