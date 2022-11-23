using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class InsertDataWebPackageFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "FunctionGroup");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Function");

            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s16-insert-web-package-function.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "FunctionGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "Function",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
