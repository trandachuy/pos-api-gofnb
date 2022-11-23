using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class RemoveStaffType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_StaffType_StaffTypeId",
                table: "Staff");

            migrationBuilder.DropTable(
                name: "StaffType");

            migrationBuilder.DropIndex(
                name: "IX_Staff_StaffTypeId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "StaffTypeId",
                table: "Staff");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StaffTypeId",
                table: "Staff",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StaffType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EnumValue = table.Column<int>(type: "int", nullable: false),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staff_StaffTypeId",
                table: "Staff",
                column: "StaffTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_StaffType_StaffTypeId",
                table: "Staff",
                column: "StaffTypeId",
                principalTable: "StaffType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
