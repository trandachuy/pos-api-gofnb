using GoFoodBeverage.Models.Address;
using System;

namespace GoFoodBeverage.Models.Supplier
{
    public class SupplierModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedTime { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? AddressId { get; set; }

        public virtual AddressModel Address { get; set; }
    }
}
