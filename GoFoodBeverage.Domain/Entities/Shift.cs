using System;
using System.ComponentModel;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Base;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Shift))]
    public class Shift : BaseEntity
    {
        [Description("The database generates a value when a row is inserted")]
        [MaxLength(15)]
        public string Code { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? StaffId { get; set; }

        public decimal InitialAmount { get; set; }

        public decimal WithdrawalAmount { get; set; }

        public DateTime? CheckInDateTime { get; set; }

        public DateTime? CheckOutDateTime { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Staff Staff { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Order> Orders { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<MaterialInventoryChecking> MaterialInventoryCheckings { get; set; }
    }
}
