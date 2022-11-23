using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateDataPlatformIdInOrderTableIsPos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [Order]
                SET PlatformId = '6C626154-5065-616C-7466-6F7200000008'
                WHERE PlatformId IS NULL
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
