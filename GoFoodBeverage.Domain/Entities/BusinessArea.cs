using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(BusinessArea))]
    public class BusinessArea : BaseEntity
    {
        [MaxLength(50)]
        public string Title { get; set; }


        public virtual ICollection<Store> Stores { get; set; }
    }
}
