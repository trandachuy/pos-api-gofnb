using System;
using System.ComponentModel;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StaffActivity))]
    public class StaffActivity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid StaffId { get; set; }

        [Description("This column is used for querying employee actions by the store.")]
        public Guid StoreId { get; set; }

        [Description("The object's name on which the user performed the action. eg: Order, product and so on")]
        public EnumActionGroup ActionGroup { get; set; }

        [Description("Action that the user performed on the website page. eg: Create, Edit and so on")]
        public EnumActionType ActionType { get; set; }

        public DateTime ExecutedTime { get; set; }

        [Description("This column will save the Id of the object on which the employee performs the activity")]
        public Guid ObjectId { get; set; }

        [Description("This column will save the Name/Code of the object on which the employee performs the activity")]
        public string ObjectName { get; set; }

        [Description("This column will save the thumbnail of the object on which the employee performs the activity")]
        public string ObjectThumbnail { get; set; }

        public virtual Store Store { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
