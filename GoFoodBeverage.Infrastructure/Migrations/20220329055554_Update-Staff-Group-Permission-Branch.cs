using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateStaffGroupPermissionBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_GroupPermission_GroupPermissionId",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_StoreBranch_StoreBranchId",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_GroupPermissionId",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_StoreBranchId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "StoreBranchId",
                table: "Staff");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissionBranches_StoreBranchId",
                table: "GroupPermissionBranches",
                column: "StoreBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermissionBranches_StoreBranch_StoreBranchId",
                table: "GroupPermissionBranches",
                column: "StoreBranchId",
                principalTable: "StoreBranch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermissionBranches_StoreBranch_StoreBranchId",
                table: "GroupPermissionBranches");

            migrationBuilder.DropIndex(
                name: "IX_GroupPermissionBranches_StoreBranchId",
                table: "GroupPermissionBranches");

            migrationBuilder.AddColumn<Guid>(
                name: "StoreBranchId",
                table: "Staff",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_GroupPermissionId",
                table: "Staff",
                column: "GroupPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_StoreBranchId",
                table: "Staff",
                column: "StoreBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_GroupPermission_GroupPermissionId",
                table: "Staff",
                column: "GroupPermissionId",
                principalTable: "GroupPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_StoreBranch_StoreBranchId",
                table: "Staff",
                column: "StoreBranchId",
                principalTable: "StoreBranch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
