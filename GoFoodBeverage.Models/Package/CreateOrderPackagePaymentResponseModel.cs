
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.Package
{
    public class CreateOrderPackagePaymentResponseModel
    {
        public int PackageCode { get; set; }

        public string PackageName { get; set; }

        public string Duration { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }

        public EnumPackagePaymentMethod PackagePaymentMethodId { get; set; }

        public AccountTransferModel AccountTransfer { get; set; }
    }

    public class AccountTransferModel
    {
        public string AccountOwner { get; set; }

        public string AccountNumber { get; set; }

        public string BankName { get; set; }

        public string Branch { get; set; }

        public string Content { get; set; }
    }
}
