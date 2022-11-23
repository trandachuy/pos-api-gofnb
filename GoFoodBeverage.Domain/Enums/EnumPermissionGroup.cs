
using System;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumPermissionGroup
    {
        Product = 1,

        Material,

        Category,

        Supplier,

        PurchaseOrder,

        Transfer,

        Customer,

        Promotion,

        AreaAndTable,

        Fee,

        Tax,

        Shift,

        Order,

        POS,
        
        WEB,

        QRCode,

        InventoryHistory
    }

    public static class EnumPermissionGroupExtensions
    {
        public static Guid ToGuid(this EnumPermissionGroup enums) => enums switch
        {
            EnumPermissionGroup.Product => new Guid("6C626154-5065-7265-6D69-737300000001"),
            EnumPermissionGroup.Material => new Guid("6C626154-5065-7265-6D69-737300000002"),
            EnumPermissionGroup.Category => new Guid("6C626154-5065-7265-6D69-737300000003"),
            EnumPermissionGroup.Supplier => new Guid("6C626154-5065-7265-6D69-737300000004"),
            EnumPermissionGroup.PurchaseOrder => new Guid("6C626154-5065-7265-6D69-737300000005"),
            EnumPermissionGroup.Transfer => new Guid("6C626154-5065-7265-6D69-737300000006"),
            EnumPermissionGroup.Customer => new Guid("6C626154-5065-7265-6D69-737300000007"),
            EnumPermissionGroup.Promotion => new Guid("6C626154-5065-7265-6D69-737300000008"),
            EnumPermissionGroup.AreaAndTable => new Guid("6C626154-5065-7265-6D69-737300000009"),
            EnumPermissionGroup.Fee => new Guid("6C626154-5065-7265-6D69-73730000000A"),
            EnumPermissionGroup.Tax => new Guid("6C626154-5065-7265-6D69-73730000000B"),
            EnumPermissionGroup.Shift => new Guid("6C626154-5065-7265-6D69-73730000000C"),
            EnumPermissionGroup.Order => new Guid("6C626154-5065-7265-6D69-73730000000D"),
            EnumPermissionGroup.POS => new Guid("6C626154-5065-7265-6D69-73730000000E"),
            EnumPermissionGroup.WEB => new Guid("6C626154-5065-7265-6D69-73730000000F"),
            EnumPermissionGroup.QRCode => new Guid("6C626154-5065-7265-6D69-737300000010"),
            EnumPermissionGroup.InventoryHistory => new Guid("6C626154-5065-7265-6D69-737300000011"),
            _ => new Guid("00000000-0000-0000-0000-000000000000")
        };
    }
}
