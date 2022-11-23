using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class InitData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /// Init AccountType
            migrationBuilder.InsertData(
               table: "AccountType",
               columns: new[] { "Id", "EnumValue", "Title", "LastSavedUser", "LastSavedTime", "CreatedUser", "CreatedTime" },
               values: new object[,] {
                    { "07dcc545-9822-489f-abec-69c1d210ea68",(int)EnumAccountType.User, EnumAccountType.User.GetDescription(), null,null,null,null},
                    { "c3408968-2942-4085-959d-a0ec09bb3952",(int)EnumAccountType.Staff, EnumAccountType.Staff.GetDescription(), null,null,null,null}
               }
           );

            /// Dumping data for table `Country`
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/countries.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
            /// Dumping data for table `City`
            sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/cities-vn.sql");
            script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            /// Dumping data for table `District`
            sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/districts-vn.sql");
            script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            /// Dumping data for table `Ward`
            sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/wards-vn.sql");
            script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            /// Dumping data for table `currency`
            sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/StaticData/currencies.sql");
            script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            /// Init BusinessArea
            migrationBuilder.InsertData(
                            table: "BusinessArea",
                            columns: new[] { "Id", "Title", "LastSavedUser", "LastSavedTime", "CreatedUser", "CreatedTime" },
                            values: new object[,] {
                                { "11dcc545-9822-489f-abec-69c1d210ea68","Restaurant", null,null,null,null},
                                { "22408968-2942-4085-959d-a0ec09bb3952","Coffee shop", null,null,null,null},
                                { "33408968-2942-4085-959d-a0ec09bb3952","Both", null,null,null,null}
                            }
                        );

            /// Init StaffType
            migrationBuilder.InsertData(
                            table: "StaffType",
                            columns: new[] { "Id", "EnumValue", "Title", "LastSavedUser", "LastSavedTime", "CreatedUser", "CreatedTime" },
                            values: new object[,] {
                                { "00dcc545-9822-489f-abec-69c1d210ea68",(int)EnumStaffType.Staff, EnumStaffType.Staff.GetDescription(), null,null,null,null},
                                { "11408968-2942-4085-959d-a0ec09bb3952",(int)EnumStaffType.Admin, EnumStaffType.Admin.GetDescription(), null,null,null,null}
                            }
                        );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
