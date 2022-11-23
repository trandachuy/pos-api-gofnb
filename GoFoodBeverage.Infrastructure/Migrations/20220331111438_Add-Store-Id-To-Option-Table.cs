using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddStoreIdToOptionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "Option",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Option_StoreId",
                table: "Option",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Option_Store_StoreId",
                table: "Option",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Option_Store_StoreId",
                table: "Option");

            migrationBuilder.DropIndex(
                name: "IX_Option_StoreId",
                table: "Option");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Option");
        }
    }
}
