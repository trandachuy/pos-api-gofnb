using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Currency))]
    public class Currency : BaseEntity
    {
        [Column(TypeName = "varchar(50)")]
        public string CurrencyName { get; set; }

        [MaxLength(3)]
        public string Code { get; set; }

        [MaxLength(5)]
        public string Symbol { get; set; }


        public virtual ICollection<Store> Stores { get; set; }
    }
}
