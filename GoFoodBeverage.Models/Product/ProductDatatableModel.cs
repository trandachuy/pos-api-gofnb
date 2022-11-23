using System;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Models.Common;

namespace GoFoodBeverage.Models.Product
{
    public class ProductDatatableModel
    {
        public int No { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Thumbnail { get; set; }

        public string Description { get; set; }

        public StatusModel Status { get; set; }

        public IEnumerable<ProductPriceModel> ProductPrices { get; set; }
        
        public IEnumerable<ChannelModel> Channels { get; set; }

        public IEnumerable<PlatformModel> Platforms { get; set; }
    }
}
