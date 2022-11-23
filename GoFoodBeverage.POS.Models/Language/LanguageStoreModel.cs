using GoFoodBeverage.POS.Models.Common;
using System;
using GoFoodBeverage.POS.Models.Store;

namespace GoFoodBeverage.POS.Models.Language
{
    public class LanguageStoreModel
    {
        public Guid Id { get; set; }

        public Guid LanguageId { get; set; }

        public Guid StoreId { get; set; }

        public StatusModel IsPublish { get; set; }

        public virtual LanguageModel Language { get; set; }

        public virtual StoreBranchModel Store { get; set; }

    }
}
