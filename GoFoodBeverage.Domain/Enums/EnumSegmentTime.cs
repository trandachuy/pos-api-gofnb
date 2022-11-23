using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumSegmentTime
    {
        /// <summary>
        /// The period is today
        /// </summary>
        Today = 0,

        /// <summary>
        /// The period is yesterday
        /// </summary>
        Yesterday = 1,

        /// <summary>
        /// The period is thisWeek
        /// </summary>
        ThisWeek = 2,

        /// <summary>
        /// The period is lastWeek
        /// </summary>
        LastWeek = 3,

        /// <summary>
        /// The period is thisMonth
        /// </summary>
        ThisMonth = 4,

        /// <summary>
        /// The period is lastMonth
        /// </summary>
        LastMonth = 5,

        /// <summary>
        /// The period is thisYear
        /// </summary>
        ThisYear = 6,

        /// <summary>
        /// The period is user custom option
        /// </summary>
        Custom = 7
    }
}
