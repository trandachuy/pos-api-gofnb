using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddInternalAccountTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternalAccount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalAccount", x => x.Id);
                });

            /// Generate static data
            /// Username: gofnb_internaltool
            /// Passowrd: GoFnBInt3rnalT00l@2022
            migrationBuilder.InsertData(
              table: "InternalAccount",
              columns: new[] { "Id", "Username", "Password" },
              values: new object[,] {
                    {"9a40c8ea-3202-43a4-90cf-8d8a561de944" , "gofnb_internaltool", "AQAAAAEAACcQAAAAEOLt8c/iDOZ2hAuyFmsgKD+TPdhuPh6mLDFTrgNEBXpSzMAj2rFt/X/37OLL1koSjQ=="},
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternalAccount");
        }
    }
}
