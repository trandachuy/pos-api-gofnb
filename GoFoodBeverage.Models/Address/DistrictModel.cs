using System;

namespace GoFoodBeverage.Models.Address
{
    public class DistrictModel
    {
        public Guid? Id { get; set; }

        public Guid CityId { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }
    }
}
