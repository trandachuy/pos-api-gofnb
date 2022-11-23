using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateComboPricingProductPriceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComboPricingProductPrice_ComboPricing_ComboPricingId",
                table: "ComboPricingProductPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_ComboPricingProductPrice_ProductPrice_ProductPriceId",
                table: "ComboPricingProductPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComboPricingProductPrice",
                table: "ComboPricingProductPrice");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductPriceId",
                table: "ComboPricingProductPrice",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ComboPricingId",
                table: "ComboPricingProductPrice",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ComboPricingProductPrice",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComboPricingProductPrice",
                table: "ComboPricingProductPrice",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ComboPricingProductPrice_ComboPricingId",
                table: "ComboPricingProductPrice",
                column: "ComboPricingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComboPricingProductPrice_ComboPricing_ComboPricingId",
                table: "ComboPricingProductPrice",
                column: "ComboPricingId",
                principalTable: "ComboPricing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComboPricingProductPrice_ProductPrice_ProductPriceId",
                table: "ComboPricingProductPrice",
                column: "ProductPriceId",
                principalTable: "ProductPrice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComboPricingProductPrice_ComboPricing_ComboPricingId",
                table: "ComboPricingProductPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_ComboPricingProductPrice_ProductPrice_ProductPriceId",
                table: "ComboPricingProductPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComboPricingProductPrice",
                table: "ComboPricingProductPrice");

            migrationBuilder.DropIndex(
                name: "IX_ComboPricingProductPrice_ComboPricingId",
                table: "ComboPricingProductPrice");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ComboPricingProductPrice");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductPriceId",
                table: "ComboPricingProductPrice",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ComboPricingId",
                table: "ComboPricingProductPrice",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComboPricingProductPrice",
                table: "ComboPricingProductPrice",
                columns: new[] { "ComboPricingId", "ProductPriceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ComboPricingProductPrice_ComboPricing_ComboPricingId",
                table: "ComboPricingProductPrice",
                column: "ComboPricingId",
                principalTable: "ComboPricing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComboPricingProductPrice_ProductPrice_ProductPriceId",
                table: "ComboPricingProductPrice",
                column: "ProductPriceId",
                principalTable: "ProductPrice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
