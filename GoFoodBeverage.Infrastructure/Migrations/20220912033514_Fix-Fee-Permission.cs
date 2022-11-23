using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class FixFeePermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s14-fix-pos-package-funtion-permission.sql");
            var script = System.IO.File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
