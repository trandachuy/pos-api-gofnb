using System;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Models.Common;
using GoFoodBeverage.Models.Unit;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Option;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Models.Tax;

namespace GoFoodBeverage.Models.Product
{
    public class ProductEditResponseModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<ProductPriceModel> ProductPrices { get; set; }

        public string Thumbnail { get; set; }

        public UnitModel Unit { get; set; }

        public TaxTypeModel Tax { get; set; }

        public Guid? ProductCategoryId { get; set; }

        public bool IsTopping { get; set; }

        public string ProductCategoryName { get; set; }

        public int StatusId { get; set; }

        public IEnumerable<ProductInventoryData> ProductInventoryData { get; set; }

        public IEnumerable<Guid> ListOptionIds { get; set; }

        public IEnumerable<OptionModel> Options { get; set; }

        public IEnumerable<UnitModel> Units { get; set; }

        public IEnumerable<TaxTypeModel> Taxes { get; set; }

        public IEnumerable<MaterialModel> Materials { get; set; }

        public IEnumerable<ProductCategoryModel> AllProductCategories { get; set; }

        public IEnumerable<Guid> ListPlatformIds { get; set; }

        public IEnumerable<PlatformModel> Platforms { get; set; }

        public IEnumerable<Guid> ProductToppingIds { get; set; }
    }

    public class ProductInventoryData
    {
        public string PriceName { get; set; }

        public IEnumerable<ProductInventoryTableData> ListProductPriceMaterial { get; set; }

        public decimal TotalCost { get; set; }
    }

    public class ProductInventoryTableData
    {
        public Guid Key { get; set; }

        public string Material { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; }

        public decimal UnitCost { get; set; }

        public decimal Cost { get; set; }
    }
}

