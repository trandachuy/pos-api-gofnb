using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Customer))]
    public class Customer : BaseEntity
    {
        public Guid? AccountId { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? PlatformId { get; set; }

        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Gender { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }

        public string Email { get; set; }

        public string Note { get; set; }

        public Guid? AddressId { get; set; }

        public EnumCustomerStatus Status { get; set; }
        
        public string Thumbnail { get; set; }

        public virtual Platform Platform { get; set; }

        public virtual Address Address { get; set; }

        public virtual Account Account { get; set; }

        public virtual Store Store { get; set; }

        public virtual ICollection<CustomerCustomerSegment> CustomerCustomerSegments { get; set; }

        public virtual CustomerPoint CustomerPoint { get; set; }

        public virtual StoreBranch Branch { get; set; }
    }
}
