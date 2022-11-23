using System;
using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumChannel
    {
        [Description("In-store")]
        InStore = 0,

        [Description("Grab food")]
        Grab = 1,
    }

    public static class EnumChannelExtensions
    {
        public static Guid ToGuid(this EnumChannel enums) => enums switch
        {
            EnumChannel.InStore => new Guid("6C626154-4365-6168-6E6E-656C00000000"),
            EnumChannel.Grab => new Guid("6C626154-4365-6168-6E6E-656C00000001"),
            _ => new Guid("00000000-0000-0000-0000-000000000000")
        };
    }
}
