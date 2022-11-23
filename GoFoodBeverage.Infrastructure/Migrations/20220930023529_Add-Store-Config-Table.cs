using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddStoreConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "The configure related to store"),
                    CurrentMaxPurchaseOrderCode = table.Column<int>(type: "int", nullable: false, comment: "Current maximum code of the purchase order belongs to store. The initial default is 1."),
                    CurrentMaxOrderCode = table.Column<int>(type: "int", nullable: false, comment: "Current maximum code of the order belongs to store. The initial default is 1."),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreConfig", x => x.Id);
                });

            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s15-update-purchase-order-code.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreConfig");
        }
    }
}
