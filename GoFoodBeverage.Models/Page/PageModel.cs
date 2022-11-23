using System;

namespace GoFoodBeverage.Models.Page
{
    public class PageModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? StoreId { get; set; }

        public string PageName { get; set; }

        public string PageContent { get; set; }

        public bool IsActive { get; set; }

        public Guid? LastSavedUser { get; set; }

        public DateTime? LastSavedTime { get; set; }

        public Guid? CreatedUser { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
