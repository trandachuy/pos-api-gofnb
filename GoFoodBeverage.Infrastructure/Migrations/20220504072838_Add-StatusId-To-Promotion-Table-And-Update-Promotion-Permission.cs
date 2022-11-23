using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddStatusIdToPromotionTableAndUpdatePromotionPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /// Update data for table `permissions`
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/permissions.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Promotion",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Promotion");
        }
    }
}
