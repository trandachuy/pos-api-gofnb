namespace GoFoodBeverage.Models.EmailCampaign
{
    public class EmailCampaignDetailModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string ButtonUrl { get; set; }

        public int Position { get; set; }

        public bool IsMain { get; set; }

        public string ButtonName { get; set; }
    }
}
