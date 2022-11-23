using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateFavoriteStoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "FavoriteStore",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "FavoriteStore",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "FavoriteStore",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "FavoriteStore");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "FavoriteStore");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "FavoriteStore");
        }
    }
}
