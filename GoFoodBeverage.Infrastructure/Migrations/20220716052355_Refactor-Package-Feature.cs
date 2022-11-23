using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class RefactorPackageFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackagePackagePermission");

            migrationBuilder.DropTable(
                name: "PackagePermissionMapping");

            migrationBuilder.DropTable(
                name: "PackagePermission");

            migrationBuilder.DropTable(
                name: "PackagePermissionGroup");

            migrationBuilder.CreateTable(
                name: "FunctionGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Function",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FunctionGroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Function", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Function_FunctionGroup_FunctionGroupId",
                        column: x => x.FunctionGroupId,
                        principalTable: "FunctionGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "FunctionPermission",
                columns: table => new
                {
                    FunctionId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionPermission", x => new { x.FunctionId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_FunctionPermission_Function_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "Function",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FunctionPermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Function_FunctionGroupId",
                table: "Function",
                column: "FunctionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionPermission_PermissionId",
                table: "FunctionPermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageFunctionGroup_FunctionGroupId",
                table: "PackageFunctionGroup",
                column: "FunctionGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FunctionPermission");

            migrationBuilder.DropTable(
                name: "PackageFunctionGroup");

            migrationBuilder.DropTable(
                name: "Function");

            migrationBuilder.DropTable(
                name: "FunctionGroup");

            migrationBuilder.CreateTable(
                name: "PackagePermissionGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackagePermissionGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackagePermissionMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackagePermissionMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackagePermissionMapping_Package_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Package",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackagePermissionMapping_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackagePermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackagePermissionGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackagePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackagePermission_PackagePermissionGroup_PackagePermissionGroupId",
                        column: x => x.PackagePermissionGroupId,
                        principalTable: "PackagePermissionGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackagePackagePermission",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackagePackagePermission", x => new { x.PackageId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_PackagePackagePermission_Package_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Package",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackagePackagePermission_PackagePermission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "PackagePermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackagePackagePermission_PermissionId",
                table: "PackagePackagePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PackagePermission_PackagePermissionGroupId",
                table: "PackagePermission",
                column: "PackagePermissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PackagePermissionMapping_PackageId",
                table: "PackagePermissionMapping",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PackagePermissionMapping_PermissionId",
                table: "PackagePermissionMapping",
                column: "PermissionId");
        }
    }
}
