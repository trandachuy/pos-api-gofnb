using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class updateplatformtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/platform-update.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
