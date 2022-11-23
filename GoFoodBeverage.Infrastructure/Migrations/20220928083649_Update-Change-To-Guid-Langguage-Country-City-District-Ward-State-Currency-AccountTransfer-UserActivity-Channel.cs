using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateChangeToGuidLangguageCountryCityDistrictWardStateCurrencyAccountTransferUserActivityChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /// Table: Langguage, Country, City, District, Ward, State, Currency, AccountTransfer, UserActivity, Channel
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s15-update-change-db-to-guid-v1.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Language",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Language",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Language",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Language",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
            name: "CreatedTime",
            table: "Country",
            type: "datetime2",
            nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Country",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "City",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "City",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "City",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "City",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "District",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "District",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "District",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "District",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Ward",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Ward",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Ward",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Ward",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "State",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "State",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "State",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "State",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Currency",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Currency",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Currency",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Currency",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "UserActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "UserActivity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "UserActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "UserActivity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Channel",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUser",
                table: "Channel",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedTime",
                table: "Channel",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastSavedUser",
                table: "Channel",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Language");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Language");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Language");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Language");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "City");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "City");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "City");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "City");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "District");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "District");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "District");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "District");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Ward");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Ward");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Ward");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Ward");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "State");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "State");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "State");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "State");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "UserActivity");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "UserActivity");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "UserActivity");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "UserActivity");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Channel");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Channel");

            migrationBuilder.DropColumn(
                name: "LastSavedTime",
                table: "Channel");

            migrationBuilder.DropColumn(
                name: "LastSavedUser",
                table: "Channel");

        }
    }
}
