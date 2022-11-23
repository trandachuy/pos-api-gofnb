using System.Collections.Generic;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumStampType
    {
        /// <summary>
        /// 40x25mm
        /// </summary>
        mm40x25 = 0,

        /// <summary>
        /// 50x30mm
        /// </summary>
        mm50x30 = 1,

        /// <summary>
        /// 50x40mm
        /// </summary>
        mm50x40 = 2,
    }

    public static class EnumStampTypeExtensions
    {
        public static string GetName(this EnumStampType enums) => enums switch
        {
            EnumStampType.mm40x25 => "40x25mm",
            EnumStampType.mm50x30 => "50x30mm",
            EnumStampType.mm50x40 => "50x40mm",
            _ => string.Empty
        };

        public static List<StampType> GetList(this EnumStampType enums)
        {
            return new List<StampType>()
           {
               new StampType
               {
                   Id = EnumStampType.mm40x25,
                   Name = EnumStampType.mm40x25.GetName()
               },
               new StampType
               {
                   Id = EnumStampType.mm50x30,
                   Name = EnumStampType.mm50x30.GetName()
               },
               new StampType
               {
                   Id = EnumStampType.mm50x40,
                   Name = EnumStampType.mm50x40.GetName()
               }
           };
        }
    }

    public class StampType
    {
        public EnumStampType Id { get; set; }

        public string Name { get; set; }
    }
}
