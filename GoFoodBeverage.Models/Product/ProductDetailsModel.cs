using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Product
{
    public class ProductDetailsModel
    {
        public GeneralReponseModel General { get; set; }
        public IQueryable<PriceReponseModel> Prices { get; set; }
        public IQueryable<OptionReponseModel> Options { get; set; }
    }

    public class GeneralReponseModel
    {
        public string ProductName { get; set; }

        public string Description { get; set; }

        public IQueryable<string> Category { get; set; }

        public string ImgUrl { get; set; }

        public string Unit { get; set; }
    }

    public class PriceReponseModel
    {
        public decimal PriceValue { get; set; }

        public string PriceName { get; set; }
    }

    public class OptionReponseModel
    {
        public Guid OptionId { get; set; }
        public string OptionName { get; set; }
        public string OptionLevelName { get; set; }
        public IQueryable<string> OptionLevel { get; set; }
    }
}
