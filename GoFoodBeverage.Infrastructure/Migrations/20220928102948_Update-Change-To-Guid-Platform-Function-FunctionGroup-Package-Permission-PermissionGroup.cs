using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateChangeToGuidPlatformFunctionFunctionGroupPackagePermissionPermissionGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /// Table: Platform, Function, FunctionGroup, Package, Permission, PermissionGroup
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s15-update-change-db-to-guid-v2.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Platform",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Platform",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Platform",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Platform",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "PermissionGroup",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "PermissionGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "PermissionGroup",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "PermissionGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Permission",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Permission",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Permission",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Permission",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Package",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Package",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Package",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Package",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "FunctionGroup",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "FunctionGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "FunctionGroup",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "FunctionGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Function",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Function",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Function",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Function",
                type: "uniqueidentifier",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Platform");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Platform");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Platform");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Platform");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "PermissionGroup");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "PermissionGroup");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "PermissionGroup");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "PermissionGroup");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "FunctionGroup");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "FunctionGroup");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "FunctionGroup");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "FunctionGroup");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Function");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Function");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Function");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Function");

        }
    }
}
