using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomerSegmentConditionDataModel
    {
        public Guid? Id { get; set; }

        public Guid? CustomerSegmentId { get; set; }

        public int? ObjectiveId { get; set; }

        public int? CustomerDataId { get; set; }

        public int? OrderDataId { get; set; }

        public int? RegistrationDateConditionId { get; set; }

        public DateTime? RegistrationTime { get; set; }

        public int? Birthday { get; set; }

        public bool? IsMale { get; set; } = false;

        public Guid? TagId { get; set; }
    }
}
