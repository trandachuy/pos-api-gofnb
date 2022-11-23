using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Area
{
    public class AreaTableByIdModel
    {
        public Guid Id { get; set; }

        public Guid? AreaId { get; set; }

        public bool IsActive { get; set; }

        public List<AreaTableDto> Tables { get; set; }

        public class AreaTableDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int NumberOfSeat { get; set; }
        }
    }
}
