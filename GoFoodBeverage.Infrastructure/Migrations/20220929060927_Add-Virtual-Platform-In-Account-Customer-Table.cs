using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddVirtualPlatformInAccountCustomerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customer_PlatformId",
                table: "Customer",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_PlatformId",
                table: "Account",
                column: "PlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Platform_PlatformId",
                table: "Account",
                column: "PlatformId",
                principalTable: "Platform",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Platform_PlatformId",
                table: "Customer",
                column: "PlatformId",
                principalTable: "Platform",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                UPDATE [Account]
                SET PlatformId = '6C626154-5065-616C-7466-6F7200000009'
                WHERE [NationalPhoneNumber] IS NOT NULL
                    AND [PhoneNumber] IS NOT NULL
                GO
                UPDATE c
                SET c.PlatformId = '6C626154-5065-616C-7466-6F7200000009'
                FROM [Customer] c
                INNER JOIN [dbo].[Account] a ON c.AccountId = a.Id
                WHERE a.PlatformId = '6C626154-5065-616C-7466-6F7200000009'
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Platform_PlatformId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Platform_PlatformId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_PlatformId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Account_PlatformId",
                table: "Account");
        }
    }
}
