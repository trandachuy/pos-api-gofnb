using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Area
{
    public class AreaTableModel
    {
        public Guid Id { get; set; }

        public int No { get; set; }

        public string Name { get; set; }

        public int NumberOfSeat { get; set; }

        public bool IsActive { get; set; }

        public AreaModel Area { get; set; }
    }
}
