using System.Collections.Generic;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumComboType
    {
        /// <summary>
        /// Flexible combo by group of items
        /// </summary>
        Flexible = 0,

        /// <summary>
        /// Specific combo by specific items
        /// </summary>
        Specific = 1
    }

    public static class EnumComboTypeExtensions
    {
        public static string ToString(this EnumComboType enums) => enums switch
        {
            EnumComboType.Flexible => "Flexible combo by group of items",
            EnumComboType.Specific => "Specific combo by specific items",
            _ => string.Empty
        };


        public static List<ComboType> GetList(this EnumComboType enums)
        {
            return new List<ComboType>()
           {
               new ComboType
               {
                   Id = EnumComboType.Flexible,
                   Name = EnumComboType.Flexible.ToString()
               },
               new ComboType
               {
                   Id = EnumComboType.Specific,
                   Name = EnumComboType.Specific.ToString()
               }
           };
        }
    }

    public class ComboType
    {
        public EnumComboType Id { get; set; }

        public string Name { get; set; }
    }
}
