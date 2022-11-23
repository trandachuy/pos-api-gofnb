using System;

namespace GoFoodBeverage.POS.Models.Address
{
    public class WardModel
    {
        public Guid? Id { get; set; }

        public Guid DistrictId { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }
    }
}
