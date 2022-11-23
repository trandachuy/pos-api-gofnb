using GoFoodBeverage.Models.Common;
using System;
using GoFoodBeverage.Models.Store;

namespace GoFoodBeverage.Models.Language
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
