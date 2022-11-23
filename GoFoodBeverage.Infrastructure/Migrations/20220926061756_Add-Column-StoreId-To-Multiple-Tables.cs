using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddColumnStoreIdToMultipleTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "UserActivity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "StoreBranchProductCategory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "StaffGroupPermissionBranch",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "PurchaseOrderMaterial",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "PromotionProductCategory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "PromotionProduct",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "PromotionBranch",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ProductProductCategory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ProductPriceMaterial",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ProductPrice",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ProductPlatform",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ProductOption",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ProductChannel",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderSession",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderPaymentTransaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderItemTopping",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderItemRestore",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderItemOption",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderHistory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderFee",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderDelivery",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderComboProductPriceItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderComboItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderCartSession",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OptionLevel",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "GroupPermissionBranches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "FunctionGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "Function",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "FileUpload",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "FeeServingType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "FeeBranch",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "DeliveryConfigPricing",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "CustomerType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "CustomerSegmentCondition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "CustomerPoint",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "CustomerCustomerSegment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "CustomerAddress",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ComboStoreBranch",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ComboProductPrice",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ComboProductGroupProductPrice",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ComboProductGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ComboPricingProductPrice",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ComboPricing",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "AreaTable",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "AccountTransfer",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "UserActivity");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "StoreBranchProductCategory");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "StaffGroupPermissionBranch");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PurchaseOrderMaterial");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PromotionProductCategory");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PromotionProduct");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PromotionBranch");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ProductProductCategory");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ProductPriceMaterial");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ProductPrice");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ProductPlatform");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ProductOption");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ProductChannel");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderSession");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderPaymentTransaction");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderItemTopping");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderItemRestore");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderItemOption");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderHistory");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderFee");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderDelivery");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderComboProductPriceItem");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderComboItem");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderCartSession");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OptionLevel");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "GroupPermissionBranches");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "FunctionGroup");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Function");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "FileUpload");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "FeeServingType");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "FeeBranch");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "DeliveryConfigPricing");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CustomerType");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CustomerSegmentCondition");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CustomerPoint");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CustomerCustomerSegment");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CustomerAddress");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ComboStoreBranch");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ComboProductPrice");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ComboProductGroupProductPrice");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ComboProductGroup");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ComboPricingProductPrice");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ComboPricing");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "AreaTable");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "AccountTransfer");
        }
    }
}
