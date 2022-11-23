using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderPackageDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchExpiredDate",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "OrderPackage");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "OrderPackage",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The last total price of package has included tax amount",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                comment: "The status of package is: PENDING/APPROVED/CANCELED",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SellerName",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                comment: "The name of the account create order package request",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PackagePaymentMethod",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                comment: "Visa = 0 or ATM = 1 or BankTransfer = 2",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PackageOderPaymentStatus",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                comment: "Unpaid = 0 or Paid = 1",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PackageDurationByMonth",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                comment: "Number of months using the package",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrderPackageType",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                comment: "StoreActivate = 0 or BranchPurchase = 1",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "OrderPackage",
                type: "nvarchar(200)",
                nullable: true,
                comment: "Note will be updated from internal tool",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiredDate",
                table: "OrderPackage",
                type: "datetime2",
                nullable: true,
                comment: "Package expiration date",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "OrderPackage",
                type: "varchar(50)",
                nullable: true,
                comment: "The email of the account create order package request",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractId",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Contract id will be updated from internal tool",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountCode",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                comment: "Account code of the account create order package request",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "OrderPackage",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates the package is active to use");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "OrderPackage");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "OrderPackage",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The last total price of package has included tax amount");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true,
                oldComment: "The status of package is: PENDING/APPROVED/CANCELED");

            migrationBuilder.AlterColumn<string>(
                name: "SellerName",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true,
                oldComment: "The name of the account create order package request");

            migrationBuilder.AlterColumn<int>(
                name: "PackagePaymentMethod",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Visa = 0 or ATM = 1 or BankTransfer = 2");

            migrationBuilder.AlterColumn<int>(
                name: "PackageOderPaymentStatus",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unpaid = 0 or Paid = 1");

            migrationBuilder.AlterColumn<int>(
                name: "PackageDurationByMonth",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of months using the package");

            migrationBuilder.AlterColumn<int>(
                name: "OrderPackageType",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "StoreActivate = 0 or BranchPurchase = 1");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "OrderPackage",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true,
                oldComment: "Note will be updated from internal tool");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiredDate",
                table: "OrderPackage",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Package expiration date");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "OrderPackage",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true,
                oldComment: "The email of the account create order package request");

            migrationBuilder.AlterColumn<string>(
                name: "ContractId",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Contract id will be updated from internal tool");

            migrationBuilder.AlterColumn<int>(
                name: "AccountCode",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Account code of the account create order package request");

            migrationBuilder.AddColumn<DateTime>(
                name: "BranchExpiredDate",
                table: "OrderPackage",
                type: "datetime2",
                nullable: true,
                comment: "Branch expired date");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "OrderPackage",
                type: "nvarchar(50)",
                nullable: true);
        }
    }
}
