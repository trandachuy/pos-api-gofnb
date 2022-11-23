using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(State))]
    public class State : BaseEntity
    {
        [Column(TypeName = "varchar(5)")]
        public string CountryCode { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string Code { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }


        public virtual ICollection<Address> Addresses { get; set; }
    }
}
