using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddBillConfigurationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BillFrameSize = table.Column<int>(type: "int", nullable: false),
                    IsShowLogo = table.Column<bool>(type: "bit", nullable: false),
                    LogoData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsShowAddress = table.Column<bool>(type: "bit", nullable: false),
                    IsShowOrderTime = table.Column<bool>(type: "bit", nullable: false),
                    IsShowCashierName = table.Column<bool>(type: "bit", nullable: false),
                    IsShowCustomerName = table.Column<bool>(type: "bit", nullable: false),
                    IsShowToping = table.Column<bool>(type: "bit", nullable: false),
                    IsShowOption = table.Column<bool>(type: "bit", nullable: false),
                    IsShowThanksMessage = table.Column<bool>(type: "bit", nullable: false),
                    ThanksMessageData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsShowWifi = table.Column<bool>(type: "bit", nullable: false),
                    WifiData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsShowPassword = table.Column<bool>(type: "bit", nullable: false),
                    PasswordData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsShowQRCode = table.Column<bool>(type: "bit", nullable: false),
                    QRCodeData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillConfiguration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillConfiguration");
        }
    }
}
