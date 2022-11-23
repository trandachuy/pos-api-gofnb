using System.Collections.Generic;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumComboPriceType
    {
        /// <summary>
        /// Fixed price
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// Specific price
        /// </summary>
        Specific = 1
    }

    public static class EnumComboPriceTypeExtensions
    {
        public static string ToString(this EnumComboPriceType enums) => enums switch
        {
            EnumComboPriceType.Fixed => "Fixed price",
            EnumComboPriceType.Specific => "Specific prices",
            _ => string.Empty
        };


        public static List<ComboPriceTypeDto> GetList(this EnumComboPriceType enums)
        {
            return new List<ComboPriceTypeDto>()
           {
               new ComboPriceTypeDto
               {
                   Id = EnumComboPriceType.Fixed,
                   Name = EnumComboPriceType.Fixed.ToString()
               },
               new ComboPriceTypeDto
               {
                   Id = EnumComboPriceType.Specific,
                   Name = EnumComboPriceType.Specific.ToString()
               }
           };
        }
    }

    public class ComboPriceTypeDto
    {
        public EnumComboPriceType Id { get; set; }

        public string Name { get; set; }
    }
}
