using GoFoodBeverage.Common.Attributes;
using System;
using System.ComponentModel;

namespace GoFoodBeverage.Common.Extensions
{
    public static class AttributeExtensions
    {
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type.ToString(), nameof(enumerationValue)");
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        public static string GetDefaultValue<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type.ToString(), nameof(enumerationValue)");
            }

            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (attrs.Length > 0)
                {
                    return ((DefaultValueAttribute)attrs[0]).Value.ToString();
                }
            }

            return enumerationValue.ToString();
        }
    }
}
