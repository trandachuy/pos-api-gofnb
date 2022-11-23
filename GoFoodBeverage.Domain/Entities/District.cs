using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(District))]
    public class District : BaseEntity
    {
        public Guid CityId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Prefix { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }


        public virtual ICollection<Address> Addresses { get; set; }
    }
}
