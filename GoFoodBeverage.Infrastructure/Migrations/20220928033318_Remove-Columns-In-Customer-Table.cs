using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class RemoveColumnsInCustomerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Country_CountryId",
                table: "Customer");

            migrationBuilder.Sql(@"
            IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Customer_CountryId'
                AND object_id = OBJECT_ID('[dbo].[Customer]'))
            BEGIN
                DROP INDEX [IX_Customer_CountryId] ON [dbo].[Customer]
            END
            ");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "NationalPhoneNumber",
                table: "Customer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalPhoneNumber",
                table: "Customer",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CountryId",
                table: "Customer",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Country_CountryId",
                table: "Customer",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
