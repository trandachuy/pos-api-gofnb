using System;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Supplier))]
    public class Supplier : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid? AddressId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string Description { get; set; }


        public virtual Store Store { get; set; }

        public virtual Address Address { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}