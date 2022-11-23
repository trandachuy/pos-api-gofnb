using System;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomerReportByPlatformModel
    {
        public Guid Id { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? PlatformId { get; set; }

        public string FullName { get; set; }

        public DateTime CreatedTime { get; set; }

        public PlatFormReportForCustomerModel Platform { get; set; }
    }

    public class OrderReportForCustomerModel 
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public Guid BranchId { get; set; }

        public Guid CustomerId { get; set; }

        public Guid PlatformId { get; set; }

        public DateTime CreatedTime { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public PlatFormReportForCustomerModel Platform { get; set; }
    }

    public class PlatFormReportForCustomerModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int StatusId { get; set; }
    }
}
