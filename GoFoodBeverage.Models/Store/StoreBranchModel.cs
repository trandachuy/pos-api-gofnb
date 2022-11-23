using GoFoodBeverage.Models.Address;
using System;

namespace GoFoodBeverage.Models.Store
{
    public class StoreBranchModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }

        public string PhoneNumber { get; set; }

        public int StatusId { get; set; }

        public string AddressInfo { get; set; }

        public virtual AddressModel Address { get; set; }
    }
}
