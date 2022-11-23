using System;

namespace GoFoodBeverage.Models.Store
{
    public class EditBranchModel
    {
        public Guid Id { get; set; }

        public string BranchName { get; set; }

        public int Code { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public Guid? CountryId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public Guid? StateId { get; set; }

        public Guid? CityId { get; set; }

        public Guid? DistrictId { get; set; }

        public Guid? WardId { get; set; }

        public string CityTown { get; set; }

        public string PostalCode { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }
    }
}
