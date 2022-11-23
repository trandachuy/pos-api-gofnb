using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Package))]
    public class Package : BaseEntity
    {
        public string Name { get; set; }

        public int? CostPerMonth { get; set; }

        public int Tax { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// The number available branch default of package
        /// </summary>
        public int AvailableBranchNumber { get; set; }

        public virtual ICollection<PackageFunction> PackageFunctions { get; set; }
    }
}
