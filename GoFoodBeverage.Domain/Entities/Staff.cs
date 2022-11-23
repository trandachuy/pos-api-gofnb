using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Staff))]
    public class Staff : BaseEntity
    {
        public Guid AccountId { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? GroupPermissionId { get; set; }

        [MaxLength(50)]
        public string FullName { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        public DateTime? Birthday { get; set; }

        public bool Gender { get; set; } // True: Male, False: Female

        public string Thumbnail { get; set; }

        /// <summary>
        /// A flag that marks the object whether or not deleted
        /// </summary>
        public bool IsDeleted { get; set; }

        public virtual Account Account { get; set; }

        public virtual Store Store { get; set; }

        public virtual ICollection<StaffGroupPermissionBranch> StaffGroupPermissionBranchs { get; set; }

        public virtual ICollection<MaterialInventoryChecking> MaterialInventoryCheckings { get; set; }

        public virtual ICollection<StaffActivity> StaffActivities { get; set; }
    }
}
