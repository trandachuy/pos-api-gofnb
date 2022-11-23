using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Page))]
    public class Page : BaseEntity
    {
        public Guid? StoreId { get; set; }

        [MaxLength(100)]
        public string PageName { get; set; }

        public string PageContent { get; set; }

        /// <summary>
        /// A flag that marks the object whether or not deleted
        /// </summary>
        public bool IsActive { get; set; }

        public virtual Store Store { get; set; }
    }
}