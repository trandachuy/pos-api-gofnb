﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateDataPlatformIdInOrderTableIsGoFnBApp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [Order]
                SET PlatformId = '6C626154-5065-616C-7466-6F7200000009'
                WHERE PlatformId IS NULL
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
