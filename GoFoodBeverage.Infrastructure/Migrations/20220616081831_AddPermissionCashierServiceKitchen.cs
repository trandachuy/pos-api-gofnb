using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddPermissionCashierServiceKitchen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderFee_FeeId",
                table: "OrderFee",
                column: "FeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderFee_Fee_FeeId",
                table: "OrderFee",
                column: "FeeId",
                principalTable: "Fee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            /// Dumping data for table `PermissionGroup`
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/permission-groups.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            /// Dumping data for table `PermissionGroup`
            sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/permissions.sql");
            script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderFee_Fee_FeeId",
                table: "OrderFee");

            migrationBuilder.DropIndex(
                name: "IX_OrderFee_FeeId",
                table: "OrderFee");
        }
    }
}
