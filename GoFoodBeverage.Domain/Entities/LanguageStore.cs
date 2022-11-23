using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(LanguageStore))]
    public class LanguageStore : BaseEntity
    {
        public Guid LanguageId { get; set; }

        public Guid StoreId { get; set; }

        public bool IsPublish { get; set; }


        public virtual Language Language { get; set; }

        public virtual Store Store { get; set; }
    }
}
