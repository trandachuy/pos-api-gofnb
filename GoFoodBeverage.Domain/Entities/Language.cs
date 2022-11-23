using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Language))]
    public class Language : BaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Emoji { get; set; }

        [MaxLength(100)]
        public string LanguageCode { get; set; }

        [MaxLength(100)]
        public string CountryCode { get; set; }

        public bool IsDefault { get; set; }


        public virtual ICollection<LanguageStore> LanguageStores { get; set; }
    }
}
