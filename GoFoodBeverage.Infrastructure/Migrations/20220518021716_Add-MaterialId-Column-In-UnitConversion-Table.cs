using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddMaterialIdColumnInUnitConversionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaterialId",
                table: "UnitConversion",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversion_MaterialId",
                table: "UnitConversion",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitConversion_Material_MaterialId",
                table: "UnitConversion",
                column: "MaterialId",
                principalTable: "Material",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitConversion_Material_MaterialId",
                table: "UnitConversion");

            migrationBuilder.DropIndex(
                name: "IX_UnitConversion_MaterialId",
                table: "UnitConversion");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "UnitConversion");
        }
    }
}
