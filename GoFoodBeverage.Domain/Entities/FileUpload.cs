using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(FileUpload))]
    public class FileUpload : BaseEntity
    {
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string NameAlias { get; set; }

        public EnumFileUsingBy UsingById { get; set; }

        public int Type { get; set; }

        public string FileUrl { get; set; }

        public bool IsActivated { get; set; }

        public Guid? StoreId { get; set; }
    }
}
