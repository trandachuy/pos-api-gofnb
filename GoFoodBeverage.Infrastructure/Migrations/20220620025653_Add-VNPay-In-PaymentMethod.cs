using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddVNPayInPaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "PaymentMethod",
              columns: new[] { "Id", "EnumId", "Name", "Icon" },
              values: new object[,] {
                    { EnumPaymentMethod.VNPay.GetId(), (int)EnumPaymentMethod.VNPay, EnumPaymentMethod.VNPay.GetName(), EnumPaymentMethod.VNPay.GetIcon()},
              }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
