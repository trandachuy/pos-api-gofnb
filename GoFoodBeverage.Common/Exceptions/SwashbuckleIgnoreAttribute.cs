using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Common.Exceptions
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SwashbuckleIgnoreAttribute : Attribute
    {
    }
}
