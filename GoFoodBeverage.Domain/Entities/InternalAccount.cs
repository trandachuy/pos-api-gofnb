using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(InternalAccount))]
    public class InternalAccount : BaseEntity
    {
        [MaxLength(50)]
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
