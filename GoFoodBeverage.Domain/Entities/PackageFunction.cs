using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PackageFunction))]
    public class PackageFunction
    {
        [Key]
        public Guid PackageId { get; set; }

        [Key]
        public Guid FunctionId { get; set; }

        public virtual Package Package { get; set; }

        public virtual Function Function { get; set; }
    }
}
