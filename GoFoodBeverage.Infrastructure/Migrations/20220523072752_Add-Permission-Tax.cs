using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddPermissionTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /// Dumping data for table `PermissionGroup`
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/permission-groups.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            /// Dumping data for table `PermissionGroup`
            sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/permissions.sql");
            script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
