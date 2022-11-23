using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddStoreIdOnGroupPermissionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "GroupPermission",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermission_StoreId",
                table: "GroupPermission",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermission_Store_StoreId",
                table: "GroupPermission",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermission_Store_StoreId",
                table: "GroupPermission");

            migrationBuilder.DropIndex(
                name: "IX_GroupPermission_StoreId",
                table: "GroupPermission");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "GroupPermission");
        }
    }
}
