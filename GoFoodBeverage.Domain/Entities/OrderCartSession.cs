using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderCartSession))]
    public class OrderCartSession : BaseEntity
    {
        public Guid CustomerId { get; set; }

        /// <summary>
        /// This field stores the cart data that the customer has selected
        /// </summary>
        public string CartItems { get; set; }

        public Guid? StoreId { get; set; }
    }
}