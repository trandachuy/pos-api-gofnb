using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateStaffActivityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "StaffActivity");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "StaffActivity");

            migrationBuilder.DropColumn(
                name: "JsonData",
                table: "StaffActivity");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "StaffActivity");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "StaffActivity");

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectId",
                table: "StaffActivity",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "This column will save the Id of the object on which the employee performs the activity");

            migrationBuilder.AddColumn<string>(
                name: "ObjectName",
                table: "StaffActivity",
                type: "nvarchar(max)",
                nullable: true,
                comment: "This column will save the Name/Code of the object on which the employee performs the activity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "StaffActivity");

            migrationBuilder.DropColumn(
                name: "ObjectName",
                table: "StaffActivity");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "StaffActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "StaffActivity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JsonData",
                table: "StaffActivity",
                type: "nvarchar(max)",
                nullable: true,
                comment: "This column is used to catch data and show it on the front-end website as a JSON object.");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "StaffActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "StaffActivity",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
