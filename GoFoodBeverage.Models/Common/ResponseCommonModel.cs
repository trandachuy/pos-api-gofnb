using System;

namespace GoFoodBeverage.Models.Common
{
    public class ResponseCommonModel
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public Guid? Guid { get; set; }

        public int? Id { get; set; }
    }
}
