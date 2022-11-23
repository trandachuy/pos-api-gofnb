using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateStoreConfigProductCategoryCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentMaxMaterialCode",
                table: "StoreConfig",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "Current maximum code of the material belongs to store. The initial default is 1.");

            migrationBuilder.AddColumn<int>(
                name: "CurrentMaxOptionCode",
                table: "StoreConfig",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "Current maximum code of the option belongs to store. The initial default is 1.");

            migrationBuilder.AddColumn<int>(
                name: "CurrentMaxProductCategoryCode",
                table: "StoreConfig",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "Current maximum code of the product category belongs to store. The initial default is 1.");

            migrationBuilder.AddColumn<int>(
                name: "CurrentMaxToppingCode",
                table: "StoreConfig",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "Current maximum code of the topping belongs to store. The initial default is 1.");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ProductCategory",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s16-update-product-category-code.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentMaxMaterialCode",
                table: "StoreConfig");

            migrationBuilder.DropColumn(
                name: "CurrentMaxOptionCode",
                table: "StoreConfig");

            migrationBuilder.DropColumn(
                name: "CurrentMaxProductCategoryCode",
                table: "StoreConfig");

            migrationBuilder.DropColumn(
                name: "CurrentMaxToppingCode",
                table: "StoreConfig");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ProductCategory");
        }
    }
}
