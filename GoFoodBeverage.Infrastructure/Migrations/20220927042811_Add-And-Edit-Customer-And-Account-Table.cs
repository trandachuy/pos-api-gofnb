using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddAndEditCustomerAndAccountTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_CustomerType_CustomerTypeId",
                table: "Customer");

            migrationBuilder.DropTable(
                name: "CustomerType");

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Customer",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Account",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NationalPhoneNumber",
                table: "Account",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformId",
                table: "Account",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(@"
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Customer_CustomerTypeId' 
    AND object_id = OBJECT_ID('[dbo].[Customer]'))
BEGIN
    DROP INDEX [IX_Customer_CustomerTypeId] ON [dbo].[Customer]
END


declare @id uniqueidentifier, @accountTypeId uniqueidentifier

select @accountTypeId = [Id] From [dbo].[AccountType] where EnumValue = 0

declare cursorCustomer cursor FOR
SELECT Id
FROM [dbo].[Customer]
where [NationalPhoneNumber] is not null and [PhoneNumber] is not null

open cursorCustomer

fetch next from cursorCustomer
into @id

while @@FETCH_STATUS = 0
begin

declare @accountId uniqueidentifier = NEWID()
declare @fullName nvarchar(250),
@phoneNumber nvarchar(250),
@nationalPhoneNumber nvarchar(250),
@password nvarchar(250),
@countryId int,
@status int

SELECT @fullName = c.FullName, 
@phoneNumber = c.PhoneNumber, 
@nationalPhoneNumber = c.NationalPhoneNumber,
@password = c.[Password], 
@countryId = c.CountryId,
@status = c.[Status]
from [Customer] c where c.[Id] = @id

insert into [dbo].[Account](
[Id], 
[FullName], 
[PhoneNumber], 
[NationalPhoneNumber], 
[Password], 
[CountryId], 
[IsActivated], 
AccountTypeId,
EmailConfirmed
)
VALUES(
@accountId,
@fullName, 
@phoneNumber,
@nationalPhoneNumber, 
@password, 
@countryId,
@status, 
@accountTypeId,
0
)

update [Customer]
set [AccountId] = @accountId
where [Id] = @id

fetch next from cursorCustomer
into @id
end

close cursorCustomer
deallocate cursorCustomer
");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "CustomerTypeId",
                table: "Customer",
                newName: "PlatformId");

            

            migrationBuilder.CreateIndex(
                name: "IX_Customer_BranchId",
                table: "Customer",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_CountryId",
                table: "Account",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Country_CountryId",
                table: "Account",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_StoreBranch_BranchId",
                table: "Customer",
                column: "BranchId",
                principalTable: "StoreBranch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Country_CountryId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_StoreBranch_BranchId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_BranchId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Account_CountryId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "NationalPhoneNumber",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PlatformId",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "PlatformId",
                table: "Customer",
                newName: "CustomerTypeId");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Customer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CustomerTypeId",
                table: "Customer",
                column: "CustomerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_CustomerType_CustomerTypeId",
                table: "Customer",
                column: "CustomerTypeId",
                principalTable: "CustomerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
