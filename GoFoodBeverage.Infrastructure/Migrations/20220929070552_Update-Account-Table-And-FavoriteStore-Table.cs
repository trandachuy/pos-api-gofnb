using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateAccountTableAndFavoriteStoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressDetail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Lng = table.Column<double>(type: "float", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CustomerAddressTypeId = table.Column<int>(type: "int", nullable: false),
                    Possion = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAddress_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "FavoriteStore",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteStore_AccountId",
                table: "FavoriteStore",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAddress_AccountId",
                table: "AccountAddress",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteStore_Account_AccountId",
                table: "FavoriteStore",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                declare @id uniqueidentifier

                declare cursorCustomer cursor FOR
                SELECT Id
                FROM [dbo].[CustomerAddress]

                open cursorCustomer

                fetch next from cursorCustomer
                into @id

                while @@FETCH_STATUS = 0
                begin

                declare @cusId uniqueidentifier,
                @customerId uniqueidentifier,
                @accountId uniqueidentifier,
                @name nvarchar(100),
                @address nvarchar(255),
                @addressDetail nvarchar(255),
                @lat float,
                @lng float,
                @note nvarchar(255),
                @customerAddressTypeId int,
                @possion int,
                @lastSavedUser uniqueidentifier,
                @lastSavedTime datetime2(7),
                @createdUser uniqueidentifier,
                @createdTime datetime2(7),
                @storeId uniqueidentifier

                SELECT @cusId = ca.Id,
                    @customerId = c.Id,
                    @accountId = c.AccountId,
                    @name = ca.Name,
                    @address = ca.Address,
                    @addressDetail = ca.AddressDetail,
                    @lat = ca.Lat,
                    @lng = ca.Lng,
                    @note = ca.Note,
                    @customerAddressTypeId = ca.CustomerAddressTypeId,
                    @possion = ca.Possion,
                    @lastSavedUser = ca.LastSavedUser,
                    @storeId = ca.StoreId,
				    @lastSavedUser = ca.LastSavedUser,
				    @lastSavedTime = ca.LastSavedTime,
				    @createdUser = ca.CreatedUser,
				    @createdTime = ca.CreatedTime

                FROM [dbo].[CustomerAddress] ca
                INNER JOIN [dbo].[Customer] c
                    on c.Id = ca.CustomerId
                WHERE ca.id = @id

                INSERT INTO [dbo].[AccountAddress]
                    ([Id]
                    ,[AccountId]
                    ,[Name]
                    ,[Address]
                    ,[AddressDetail]
                    ,[Lat]
                    ,[Lng]
                    ,[Note]
                    ,[CustomerAddressTypeId]
                    ,[Possion]
                    ,[StoreId]
                    ,[LastSavedUser]
                    ,[LastSavedTime]
                    ,[CreatedUser]
                    ,[CreatedTime])
                 VALUES
                    (@cusId
                    ,@accountId
                    ,@name
                    ,@address
                    ,@addressDetail
                    ,@lat
                    ,@lng
                    ,@note
                    ,@customerAddressTypeId
                    ,@possion
                    ,@storeId
                    ,@lastSavedUser
                    ,@lastSavedTime
                    ,@createdUser
                    ,@createdTime)

                fetch next from cursorCustomer
                into @id
                end

                close cursorCustomer
                deallocate cursorCustomer
            ");

            migrationBuilder.Sql(@"
                declare @id uniqueidentifier

                declare cursorFavoriteStore cursor FOR
                SELECT Id
                FROM [dbo].[FavoriteStore]

                open cursorFavoriteStore

                fetch next from cursorFavoriteStore
                into @id

                while @@FETCH_STATUS = 0
                begin

                declare @customerId uniqueidentifier,
                @accountId uniqueidentifier

                SELECT @customerId = c.Id,
                    @accountId = c.AccountId

                FROM [dbo].[FavoriteStore] fs
                INNER JOIN [dbo].[Customer] c
                    on c.Id = fs.CustomerId
                WHERE fs.id = @id

                UPDATE FavoriteStore
                SET AccountId = @accountId
                WHERE CustomerId = @customerId

                fetch next from cursorFavoriteStore
                into @id
                end

                close cursorFavoriteStore
                deallocate cursorFavoriteStore
            ");

            migrationBuilder.DropTable(
                name: "CustomerAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteStore_Account_AccountId",
                table: "FavoriteStore");

            migrationBuilder.DropTable(
                name: "AccountAddress");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteStore_AccountId",
                table: "FavoriteStore");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "FavoriteStore");

            migrationBuilder.CreateTable(
                name: "CustomerAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressDetail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerAddressTypeId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Lng = table.Column<double>(type: "float", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Possion = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddress_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddress_CustomerId",
                table: "CustomerAddress",
                column: "CustomerId");
        }
    }
}
