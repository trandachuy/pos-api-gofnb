using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PackageDurationByMonth))]
    public class PackageDurationByMonth
    {
        public int Id { get; set; }

        public int Period { get; set; }
    }
}
