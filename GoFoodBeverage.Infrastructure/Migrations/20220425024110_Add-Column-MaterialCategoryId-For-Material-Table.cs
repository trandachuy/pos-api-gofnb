using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddColumnMaterialCategoryIdForMaterialTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaterialCategoryId",
                table: "Material",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Material_MaterialCategoryId",
                table: "Material",
                column: "MaterialCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Material_MaterialCategory_MaterialCategoryId",
                table: "Material",
                column: "MaterialCategoryId",
                principalTable: "MaterialCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Material_MaterialCategory_MaterialCategoryId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_MaterialCategoryId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "MaterialCategoryId",
                table: "Material");
        }
    }
}
