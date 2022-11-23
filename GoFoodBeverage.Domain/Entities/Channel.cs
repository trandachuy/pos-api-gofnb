using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Channel))]
    public class Channel : BaseEntity
    {
        public string Name { get; set; }
    }
}
