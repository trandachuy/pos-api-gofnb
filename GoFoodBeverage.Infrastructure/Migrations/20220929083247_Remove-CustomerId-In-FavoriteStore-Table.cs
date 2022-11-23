using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class RemoveCustomerIdInFavoriteStoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteStore_Customer_CustomerId",
                table: "FavoriteStore");

            migrationBuilder.Sql(@"
            IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='FK_FavoriteStore_Customer_CustomerId'
                AND object_id = OBJECT_ID('[dbo].[FavoriteStore]'))
            BEGIN
                DROP INDEX [FK_FavoriteStore_Customer_CustomerId] ON [FavoriteStore];
            END

            IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_FavoriteStore_CustomerId'
                AND object_id = OBJECT_ID('[dbo].[FavoriteStore]'))
            BEGIN
                DROP INDEX [IX_FavoriteStore_CustomerId] ON [FavoriteStore];
            END
            ");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "FavoriteStore");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "FavoriteStore",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteStore_CustomerId",
                table: "FavoriteStore",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteStore_Customer_CustomerId",
                table: "FavoriteStore",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
