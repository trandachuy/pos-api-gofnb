using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Area
{
    public class AreaTablesByBranchIdModel
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<AreaTableDto> AreaTables { get; set; }

        public class AreaTableDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }
}
