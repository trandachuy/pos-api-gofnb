using System;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumBusinessArea
    {
        Food,
        Beverages,
        Both,
    }

    public static class EnumBusinessAreaExtensions
    {
        public static Guid ToGuid(this EnumBusinessArea enums) => enums switch
        {
            EnumBusinessArea.Food => new Guid("11DCC545-9822-489F-ABEC-69C1D210EA68"),
            EnumBusinessArea.Beverages => new Guid("22408968-2942-4085-959D-A0EC09BB3952"),
            EnumBusinessArea.Both => new Guid("33408968-2942-4085-959D-A0EC09BB3952"),
            _ => new Guid("00000000-0000-0000-0000-000000000000")
        };
    }
}