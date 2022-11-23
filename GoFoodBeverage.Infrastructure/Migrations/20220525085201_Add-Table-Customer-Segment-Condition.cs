using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddTableCustomerSegmentCondition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerSegmentCondition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerSegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ObjectiveId = table.Column<int>(type: "int", nullable: true),
                    CustomerDataId = table.Column<int>(type: "int", nullable: true),
                    OrderDataId = table.Column<int>(type: "int", nullable: true),
                    RegistrationDateConditionId = table.Column<int>(type: "int", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Birthday = table.Column<int>(type: "int", nullable: true),
                    IsMale = table.Column<bool>(type: "bit", nullable: true),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSegmentCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerSegmentCondition_CustomerSegment_CustomerSegmentId",
                        column: x => x.CustomerSegmentId,
                        principalTable: "CustomerSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSegmentCondition_CustomerSegmentId",
                table: "CustomerSegmentCondition",
                column: "CustomerSegmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerSegmentCondition");
        }
    }
}
