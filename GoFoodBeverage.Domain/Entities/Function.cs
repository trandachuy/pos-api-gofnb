using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Function))]
    public class Function : BaseEntity
    {
        public Guid? FunctionGroupId { get; set; }

        public string Name { get; set; }

        public virtual FunctionGroup FunctionGroup { get; set; }

        public virtual ICollection<FunctionPermission> FunctionPermissions { get; set; }

        public virtual ICollection<PackageFunction> PackageFunctions { get; set; }
    }
}
