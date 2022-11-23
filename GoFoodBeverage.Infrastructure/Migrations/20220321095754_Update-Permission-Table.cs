using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdatePermissionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermissionPermission_GroupPermission_GroupPermissionsId",
                table: "GroupPermissionPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermissionPermission_Permission_PermissionsId",
                table: "GroupPermissionPermission");

            migrationBuilder.RenameColumn(
                name: "PermissionsId",
                table: "GroupPermissionPermission",
                newName: "PermissionId");

            migrationBuilder.RenameColumn(
                name: "GroupPermissionsId",
                table: "GroupPermissionPermission",
                newName: "GroupPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupPermissionPermission_PermissionsId",
                table: "GroupPermissionPermission",
                newName: "IX_GroupPermissionPermission_PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermissionPermission_GroupPermission_GroupPermissionId",
                table: "GroupPermissionPermission",
                column: "GroupPermissionId",
                principalTable: "GroupPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermissionPermission_Permission_PermissionId",
                table: "GroupPermissionPermission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermissionPermission_GroupPermission_GroupPermissionId",
                table: "GroupPermissionPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermissionPermission_Permission_PermissionId",
                table: "GroupPermissionPermission");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "GroupPermissionPermission",
                newName: "PermissionsId");

            migrationBuilder.RenameColumn(
                name: "GroupPermissionId",
                table: "GroupPermissionPermission",
                newName: "GroupPermissionsId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupPermissionPermission_PermissionId",
                table: "GroupPermissionPermission",
                newName: "IX_GroupPermissionPermission_PermissionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermissionPermission_GroupPermission_GroupPermissionsId",
                table: "GroupPermissionPermission",
                column: "GroupPermissionsId",
                principalTable: "GroupPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermissionPermission_Permission_PermissionsId",
                table: "GroupPermissionPermission",
                column: "PermissionsId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
