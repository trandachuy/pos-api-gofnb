using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(FunctionPermission))]
    public class FunctionPermission
    {
        [Key]
        public Guid FunctionId { get; set; }

        [Key]
        public Guid PermissionId { get; set; }

        public Function Function { get; set; }

        public Permission Permission { get; set; }
    }
}
