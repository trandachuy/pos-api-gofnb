using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class RenameTablePackageFunctionGroupToPackageFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Function_FunctionGroup_FunctionGroupId",
                table: "Function");

            migrationBuilder.DropTable(
                name: "PackageFunctionGroup");

            migrationBuilder.AlterColumn<int>(
                name: "FunctionGroupId",
                table: "Function",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "Function",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PackageFunction",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    FunctionId = table.Column<int>(type: "int", nullable: false),
                    FunctionGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageFunction", x => new { x.PackageId, x.FunctionId });
                    table.ForeignKey(
                        name: "FK_PackageFunction_Function_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "Function",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageFunction_FunctionGroup_FunctionGroupId",
                        column: x => x.FunctionGroupId,
                        principalTable: "FunctionGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageFunction_Package_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Package",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Function_PackageId",
                table: "Function",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageFunction_FunctionGroupId",
                table: "PackageFunction",
                column: "FunctionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageFunction_FunctionId",
                table: "PackageFunction",
                column: "FunctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Function_FunctionGroup_FunctionGroupId",
                table: "Function",
                column: "FunctionGroupId",
                principalTable: "FunctionGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Function_Package_PackageId",
                table: "Function",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Function_FunctionGroup_FunctionGroupId",
                table: "Function");

            migrationBuilder.DropForeignKey(
                name: "FK_Function_Package_PackageId",
                table: "Function");

            migrationBuilder.DropTable(
                name: "PackageFunction");

            migrationBuilder.DropIndex(
                name: "IX_Function_PackageId",
                table: "Function");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Function");

            migrationBuilder.AlterColumn<int>(
                name: "FunctionGroupId",
                table: "Function",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PackageFunctionGroup",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    FunctionGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageFunctionGroup", x => new { x.PackageId, x.FunctionGroupId });
                    table.ForeignKey(
                        name: "FK_PackageFunctionGroup_FunctionGroup_FunctionGroupId",
                        column: x => x.FunctionGroupId,
                        principalTable: "FunctionGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageFunctionGroup_Package_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Package",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageFunctionGroup_FunctionGroupId",
                table: "PackageFunctionGroup",
                column: "FunctionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Function_FunctionGroup_FunctionGroupId",
                table: "Function",
                column: "FunctionGroupId",
                principalTable: "FunctionGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
