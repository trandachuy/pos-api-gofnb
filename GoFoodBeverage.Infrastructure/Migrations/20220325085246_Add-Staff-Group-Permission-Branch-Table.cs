using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddStaffGroupPermissionBranchTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branchs",
                table: "StaffGroupPermissionBranch");

            migrationBuilder.DropColumn(
                name: "GroupPermissions",
                table: "StaffGroupPermissionBranch");

            migrationBuilder.CreateTable(
                name: "GroupPermissionBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffGroupPermissionBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupPermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPermissionBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPermissionBranches_GroupPermission_GroupPermissionId",
                        column: x => x.GroupPermissionId,
                        principalTable: "GroupPermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPermissionBranches_StaffGroupPermissionBranch_StaffGroupPermissionBranchId",
                        column: x => x.StaffGroupPermissionBranchId,
                        principalTable: "StaffGroupPermissionBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissionBranches_GroupPermissionId",
                table: "GroupPermissionBranches",
                column: "GroupPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissionBranches_StaffGroupPermissionBranchId",
                table: "GroupPermissionBranches",
                column: "StaffGroupPermissionBranchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupPermissionBranches");

            migrationBuilder.AddColumn<string>(
                name: "Branchs",
                table: "StaffGroupPermissionBranch",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupPermissions",
                table: "StaffGroupPermissionBranch",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
