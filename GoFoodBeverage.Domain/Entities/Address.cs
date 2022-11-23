using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Address))]
    public class Address : BaseEntity
    {
        public Guid? CountryId { get; set; }

        public Guid? StateId { get; set; } // State

        public Guid? CityId { get; set; } // Province / city / town

        public Guid? DistrictId { get; set; } // District

        public Guid? WardId { get; set; } // ward

        [MaxLength(255)]
        public string Address1 { get; set; }

        [MaxLength(255)]
        public string Address2{ get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [MaxLength(255)]
        public string CityTown { get; set; }

        [MaxLength(255)]
        public string PostalCode { get; set; } // zip / postal code


        public Country Country { get; set; }

        public State State { get; set; }

        public City City { get; set; }

        public District District { get; set; }

        public Ward Ward { get; set; }
    }
}
