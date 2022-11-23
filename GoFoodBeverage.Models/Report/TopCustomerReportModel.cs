using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Report
{
    public class TopCustomerReportModel
    {
        public int? No { get; set; }

        public Guid? Id { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Rank { get; set; }

        public decimal? Point { get; set; }

        public int? OrderNumber { get; set; }

        public decimal? TotalAmount { get; set; }

        public int? AccumulatedPoint { get; set; }

        public string Color { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
