using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddParentIdQrCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClonedByQrCodeId",
                table: "QRCode",
                type: "uniqueidentifier",
                nullable: true,
                comment: "The field has value when cloned from another qr code.");

            migrationBuilder.CreateIndex(
                name: "IX_QRCode_ClonedByQrCodeId",
                table: "QRCode",
                column: "ClonedByQrCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_QRCode_QRCode_ClonedByQrCodeId",
                table: "QRCode",
                column: "ClonedByQrCodeId",
                principalTable: "QRCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QRCode_QRCode_ClonedByQrCodeId",
                table: "QRCode");

            migrationBuilder.DropIndex(
                name: "IX_QRCode_ClonedByQrCodeId",
                table: "QRCode");

            migrationBuilder.DropColumn(
                name: "ClonedByQrCodeId",
                table: "QRCode");
        }
    }
}
