using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumAccountType
    {
        /// <summary>
        /// Account type for customer, enduser
        /// </summary>
        [Description("USER")]
        User = 0,

        /// <summary>
        /// Account type for Admin, Manager, Staff, etc... of store, store's branch
        /// </summary>
        [Description("STAFF")]
        Staff = 1,
    }
}
