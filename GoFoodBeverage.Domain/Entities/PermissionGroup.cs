using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    /// <summary>
    /// Description: This is group have permissions.
    /// Meta-data
    /// Example: Product has permissions => View product, Create product, Edit product, etc,...
    /// </summary>
    [Table(nameof(PermissionGroup))]
    public class PermissionGroup : BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }

        [Description("The order number of permission group")]
        public int? Order { get; set; }


        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
