using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(FunctionGroup))]
    public class FunctionGroup : BaseEntity
    {
        public string Name { get; set; }

        [Description("The order number of function group")]
        public int? Order { get; set; }


        public virtual ICollection<Function> Functions { get; set; }
    }
}
