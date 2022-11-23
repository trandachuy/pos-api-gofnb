using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderColumnToTableFunctionGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s16-update-order-column-table-function-group.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "PermissionGroup",
                type: "int",
                nullable: true,
                comment: "The order number of permission group",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "The priority of permission group");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "FunctionGroup",
                type: "int",
                nullable: true,
                comment: "The order number of function group",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "The priority of function group");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "PermissionGroup",
                type: "int",
                nullable: true,
                comment: "The priority of permission group",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "The order number of permission group");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "FunctionGroup",
                type: "int",
                nullable: true,
                comment: "The priority of function group",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "The order number of function group");
        }
    }
}
