using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Store
{
    public class StoreInformationModel
    {
        public string PhoneCode { get; set; }

        public string CountryIso { get; set; }

        public string CountryName { get; set; }

        public string BranchAddress { get; set; }

        public string Logo { get; set; }

        public bool IsStoreHasKitchen { get; set; }

        public bool IsAutoPrintStamp { get; set; }

        public bool IsPaymentLater { get; set; }

        public bool IsCheckProductSell { get; set; }

        public LocationDto BranchLocation { get; set; }

        public class LocationDto
        {
            public double? Lat { get; set; }

            public double? Lng { get; set; }
        }
    }
}
