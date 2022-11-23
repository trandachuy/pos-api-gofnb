using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class InitPaymentMethodData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "PaymentMethod",
              columns: new[] { "Id","EnumId", "Name", "Icon" },
              values: new object[,] {
                    { EnumPaymentMethod.MoMo.GetId(), (int)EnumPaymentMethod.MoMo, EnumPaymentMethod.MoMo.GetName(), EnumPaymentMethod.MoMo.GetIcon()},
                    { EnumPaymentMethod.ZaloPay.GetId(), (int)EnumPaymentMethod.ZaloPay, EnumPaymentMethod.ZaloPay.GetName(), EnumPaymentMethod.ZaloPay.GetIcon()},
                    { EnumPaymentMethod.CreditDebitCard.GetId(), (int)EnumPaymentMethod.CreditDebitCard, EnumPaymentMethod.CreditDebitCard.GetName(), EnumPaymentMethod.CreditDebitCard.GetIcon()},
                    { EnumPaymentMethod.Cash.GetId(), (int)EnumPaymentMethod.Cash, EnumPaymentMethod.Cash.GetName(), EnumPaymentMethod.Cash.GetIcon()},
              }
          );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
