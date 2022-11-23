using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(City))]
    public class City : BaseEntity
    {
        public Guid CountryId { get; set; }

        [MaxLength(100)]
        public string Code { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }


        public virtual ICollection<Address> Addresses { get; set; }
    }
}
