using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumActionGroup
    {
        [Description("activityObject.order")]
        Order = 1,

        [Description("activityObject.product")]
        Product = 2,

        [Description("activityObject.productCategory")]
        ProductCategory = 3,

        [Description("activityObject.option")]
        Option = 4,

        [Description("activityObject.combo")]
        Combo = 5,

        [Description("activityObject.material")]
        Material = 6,

        [Description("activityObject.materialCategory")]
        MaterialCategory = 7,

        [Description("activityObject.supplier")]
        Supplier = 8,

        [Description("activityObject.purchaseOrder")]
        PurchaseOrder = 9,

        [Description("activityObject.customer")]
        Customer = 10,

        [Description("activityObject.customerSegment")]
        CustomerSegment = 11,

        [Description("activityObject.memberShip")]
        MemberShip = 12,

        [Description("activityObject.pointConfiguration")]
        PointConfiguration = 13,

        [Description("activityObject.storeBranch")]
        StoreBranch = 14
    }
}
