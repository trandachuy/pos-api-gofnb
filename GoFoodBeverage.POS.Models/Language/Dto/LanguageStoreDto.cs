using System;

namespace GoFoodBeverage.POS.Models.Language.Dto
{
    public class LanguageStoreDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Emoji { get; set; }

        public bool IsPublish { get; set; }

        public bool IsDefault { get; set; }

        public string LanguageCode { get; set; }

        public string CountryCode { get; set; }

    }
}
