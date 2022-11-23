using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddStampConfigBarcodeConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BarcodeConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StampType = table.Column<int>(type: "int", nullable: false),
                    BarcodeType = table.Column<int>(type: "int", nullable: false),
                    IsShowName = table.Column<bool>(type: "bit", nullable: false),
                    IsShowPrice = table.Column<bool>(type: "bit", nullable: false),
                    IsShowCode = table.Column<bool>(type: "bit", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarcodeConfig_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StampConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StampType = table.Column<int>(type: "int", nullable: false),
                    IsShowLogo = table.Column<bool>(type: "bit", nullable: false),
                    IsShowTime = table.Column<bool>(type: "bit", nullable: false),
                    IsShowNumberOfItem = table.Column<bool>(type: "bit", nullable: false),
                    IsShowNote = table.Column<bool>(type: "bit", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StampConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StampConfig_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeConfig_StoreId",
                table: "BarcodeConfig",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StampConfig_StoreId",
                table: "StampConfig",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodeConfig");

            migrationBuilder.DropTable(
                name: "StampConfig");
        }
    }
}
