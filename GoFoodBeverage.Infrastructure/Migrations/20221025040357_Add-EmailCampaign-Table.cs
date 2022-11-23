using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddEmailCampaignTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailCampaign",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SendingTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Default is current date & (current time + 15 minutes)"),
                    EmailSubject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailCampaignType = table.Column<int>(type: "int", nullable: false, comment: "Option: 'Send to email address', 'Send to customer group'"),
                    EmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PrimaryColor = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    SecondaryColor = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FooterContent = table.Column<string>(type: "ntext", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailCampaign", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailCampaignCustomerSegment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmailCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerSegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailCampaignCustomerSegment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailCampaignCustomerSegment_CustomerSegment_CustomerSegmentId",
                        column: x => x.CustomerSegmentId,
                        principalTable: "CustomerSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailCampaignCustomerSegment_EmailCampaign_EmailCampaignId",
                        column: x => x.EmailCampaignId,
                        principalTable: "EmailCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailCampaignDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmailCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ButtonName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ButtonLink = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailCampaignDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailCampaignDetail_EmailCampaign_EmailCampaignId",
                        column: x => x.EmailCampaignId,
                        principalTable: "EmailCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailCampaignSocial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmailCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailCampaignSocial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailCampaignSocial_EmailCampaign_EmailCampaignId",
                        column: x => x.EmailCampaignId,
                        principalTable: "EmailCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailCampaignCustomerSegment_CustomerSegmentId",
                table: "EmailCampaignCustomerSegment",
                column: "CustomerSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailCampaignCustomerSegment_EmailCampaignId",
                table: "EmailCampaignCustomerSegment",
                column: "EmailCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailCampaignDetail_EmailCampaignId",
                table: "EmailCampaignDetail",
                column: "EmailCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailCampaignSocial_EmailCampaignId",
                table: "EmailCampaignSocial",
                column: "EmailCampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailCampaignCustomerSegment");

            migrationBuilder.DropTable(
                name: "EmailCampaignDetail");

            migrationBuilder.DropTable(
                name: "EmailCampaignSocial");

            migrationBuilder.DropTable(
                name: "EmailCampaign");
        }
    }
}
