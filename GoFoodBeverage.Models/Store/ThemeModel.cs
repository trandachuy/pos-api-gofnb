using System;

namespace GoFoodBeverage.Models.Store
{
    public class ThemeModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Tags { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }

        public string Thumbnail { get; set; }
    }
}
