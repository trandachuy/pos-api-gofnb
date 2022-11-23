using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(MaterialCategory))]
    public class MaterialCategory : BaseEntity
    {
        public Guid? StoreId { get; set; }

        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public virtual Store Store { get; set; }

        public virtual ICollection<Material> Materials { get; set; }
    }
}
