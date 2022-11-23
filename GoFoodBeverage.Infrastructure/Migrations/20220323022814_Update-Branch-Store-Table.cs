using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateBranchStoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "StoreBranch",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "StoreBranch",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StoreBranch",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "StoreBranch",
                type: "varchar(15)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "StoreBranch",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StoreBranch_AddressId",
                table: "StoreBranch",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreBranch_Address_AddressId",
                table: "StoreBranch",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreBranch_Address_AddressId",
                table: "StoreBranch");

            migrationBuilder.DropIndex(
                name: "IX_StoreBranch_AddressId",
                table: "StoreBranch");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "StoreBranch");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "StoreBranch");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "StoreBranch");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "StoreBranch");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "StoreBranch");
        }
    }
}
