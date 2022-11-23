using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddComboTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Combo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsShowAllBranches = table.Column<bool>(type: "bit", nullable: false),
                    ComboTypeId = table.Column<int>(type: "int", nullable: false),
                    ComboPriceTypeId = table.Column<int>(type: "int", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComboPricing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComboName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboPricing_Combo_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboProductGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboProductGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboProductGroup_Combo_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboProductGroup_ProductCategory_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboProductPrice",
                columns: table => new
                {
                    ComboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboProductPrice", x => new { x.ComboId, x.ProductPriceId });
                    table.ForeignKey(
                        name: "FK_ComboProductPrice_Combo_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboProductPrice_ProductPrice_ProductPriceId",
                        column: x => x.ProductPriceId,
                        principalTable: "ProductPrice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboStoreBranch",
                columns: table => new
                {
                    ComboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboStoreBranch", x => new { x.ComboId, x.BranchId });
                    table.ForeignKey(
                        name: "FK_ComboStoreBranch_Combo_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboStoreBranch_StoreBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "StoreBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboPricingProductPrice",
                columns: table => new
                {
                    ComboPricingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboPricingProductPrice", x => new { x.ComboPricingId, x.ProductPriceId });
                    table.ForeignKey(
                        name: "FK_ComboPricingProductPrice_ComboPricing_ComboPricingId",
                        column: x => x.ComboPricingId,
                        principalTable: "ComboPricing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboPricingProductPrice_ProductPrice_ProductPriceId",
                        column: x => x.ProductPriceId,
                        principalTable: "ProductPrice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboProductGroupProductPrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComboProductGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboProductGroupProductPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboProductGroupProductPrice_ComboProductGroup_ComboProductGroupId",
                        column: x => x.ComboProductGroupId,
                        principalTable: "ComboProductGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboProductGroupProductPrice_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComboProductGroupProductPrice_ProductPrice_ProductPriceId",
                        column: x => x.ProductPriceId,
                        principalTable: "ProductPrice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboPricing_ComboId",
                table: "ComboPricing",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboPricingProductPrice_ProductPriceId",
                table: "ComboPricingProductPrice",
                column: "ProductPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductGroup_ComboId",
                table: "ComboProductGroup",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductGroup_ProductCategoryId",
                table: "ComboProductGroup",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductGroupProductPrice_ComboProductGroupId",
                table: "ComboProductGroupProductPrice",
                column: "ComboProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductGroupProductPrice_ProductId",
                table: "ComboProductGroupProductPrice",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductGroupProductPrice_ProductPriceId",
                table: "ComboProductGroupProductPrice",
                column: "ProductPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductPrice_ProductPriceId",
                table: "ComboProductPrice",
                column: "ProductPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboStoreBranch_BranchId",
                table: "ComboStoreBranch",
                column: "BranchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboPricingProductPrice");

            migrationBuilder.DropTable(
                name: "ComboProductGroupProductPrice");

            migrationBuilder.DropTable(
                name: "ComboProductPrice");

            migrationBuilder.DropTable(
                name: "ComboStoreBranch");

            migrationBuilder.DropTable(
                name: "ComboPricing");

            migrationBuilder.DropTable(
                name: "ComboProductGroup");

            migrationBuilder.DropTable(
                name: "Combo");
        }
    }
}
