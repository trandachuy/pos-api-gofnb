using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Account))]
    public class Account : BaseEntity
    {
        public Guid AccountTypeId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(500)]
        public string Password { get; set; }

        [MaxLength(250)]
        public string FullName { get; set; }

        [MaxLength(50)]
        public string ValidateCode { get; set; }

        public bool EmailConfirmed { get; set; }

        public Guid? PlatformId { get; set; }

        public bool IsActivated { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(50)]
        public string NationalPhoneNumber { get; set; }

        public Guid? CountryId { get; set; }

        public string Thumbnail { get; set; }

        /// <summary>
        /// A flag that marks the object whether or not deleted
        /// </summary>
        public bool IsDeleted { get; set; }

        public virtual Platform Platform { get; set; }

        public virtual Country Country { get; set; }

        public virtual AccountType AccountType { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }

        public virtual ICollection<AccountAddress> AccountAddresses { get; set; }

        public virtual ICollection<FavoriteStore> FavoriteStores { get; set; }

        public virtual ICollection<AccountSearchHistory> AccountSearchHistories { get; set; }

    }
}
