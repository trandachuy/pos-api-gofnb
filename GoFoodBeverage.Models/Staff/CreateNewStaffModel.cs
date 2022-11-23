using System;

namespace GoFoodBeverage.Models.Staff
{
    public class CreateNewStaffRequestModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool Gender { get; set; }

        public DateTime? Birthday { get; set; }
    }
}
