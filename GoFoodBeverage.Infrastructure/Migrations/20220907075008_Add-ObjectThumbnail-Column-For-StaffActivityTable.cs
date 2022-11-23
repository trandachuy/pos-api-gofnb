using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddObjectThumbnailColumnForStaffActivityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ObjectThumbnail",
                table: "StaffActivity",
                type: "nvarchar(max)",
                nullable: true,
                comment: "This column will save the thumbnail of the object on which the employee performs the activity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectThumbnail",
                table: "StaffActivity");
        }
    }
}
