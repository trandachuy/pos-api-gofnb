using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class MultipleBranchesPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredDate",
                table: "StoreBranch",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ActivatedByOrderPackageId",
                table: "Store",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvailableBranchNumber",
                table: "Package",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "OrderPackage",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShopPhoneNumber",
                table: "OrderPackage",
                type: "varchar(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShopName",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SellerName",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "OrderPackage",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                table: "OrderPackage",
                type: "datetime2",
                nullable: false,
                comment: "The last updated date or the activate package date",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "OrderPackage",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "OrderPackage",
                type: "nvarchar(5)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BranchExpiredDate",
                table: "OrderPackage",
                type: "datetime2",
                nullable: true,
                comment: "Branch expired date");

            migrationBuilder.AddColumn<decimal>(
                name: "BranchPurchaseTotalPrice",
                table: "OrderPackage",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Price of number of branches and number of years");

            migrationBuilder.AddColumn<int>(
                name: "BranchQuantity",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Number of branches to buy");

            migrationBuilder.AddColumn<decimal>(
                name: "BranchUnitPricePerYear",
                table: "OrderPackage",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "The price of a branch per year, the value related from the store package price per year");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderBranchPurchasePackageId",
                table: "OrderPackage",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderPackageType",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainAmount",
                table: "OrderPackage",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Remain amount = (Last package price / (number of years of package * 360) ) * Remain days");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                table: "StoreBranch");

            migrationBuilder.DropColumn(
                name: "ActivatedByOrderPackageId",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "AvailableBranchNumber",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "BranchExpiredDate",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "BranchPurchaseTotalPrice",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "BranchQuantity",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "BranchUnitPricePerYear",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "OrderBranchPurchasePackageId",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "OrderPackageType",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "RemainAmount",
                table: "OrderPackage");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShopPhoneNumber",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShopName",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SellerName",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                table: "OrderPackage",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "The last updated date or the activate package date");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);
        }
    }
}
