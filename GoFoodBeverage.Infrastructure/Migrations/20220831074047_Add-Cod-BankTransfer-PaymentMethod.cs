using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddCodBankTransferPaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
             table: "PaymentMethod",
             columns: new[] { "Id", "EnumId", "Name", "Icon" },
             values: new object[,] {
                    { EnumPaymentMethod.COD.GetId(), (int)EnumPaymentMethod.COD, EnumPaymentMethod.COD.GetName(), EnumPaymentMethod.COD.GetIcon()},
                    { EnumPaymentMethod.BankTransfer.GetId(), (int)EnumPaymentMethod.BankTransfer, EnumPaymentMethod.BankTransfer.GetName(), EnumPaymentMethod.BankTransfer.GetIcon()},
             }
           );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
