﻿// <auto-generated />
using System;
using GoFoodBeverage.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    [DbContext(typeof(GoFoodBeverageDbContext))]
    [Migration("20220321030130_add-create-by-staff-id-on-table-group-permission")]
    partial class addcreatebystaffidontablegrouppermission
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.14")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ValidateCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("AccountTypeId");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.AccountType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EnumValue")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("AccountType");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address1")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Address2")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("CityId")
                        .HasColumnType("int");

                    b.Property<string>("CityTown")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("DistrictId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("StateId")
                        .HasColumnType("int");

                    b.Property<int?>("WardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("CountryId");

                    b.HasIndex("DistrictId");

                    b.HasIndex("StateId");

                    b.HasIndex("WardId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.BusinessArea", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("BusinessArea");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<double?>("Lat")
                        .HasColumnType("float");

                    b.Property<double?>("Lng")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("City");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Iso")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Iso3")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Nicename")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Numcode")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Phonecode")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("CurrencyName")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Symbol")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.HasKey("Id");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StoreId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CustomerTypeId");

                    b.HasIndex("StoreId");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.CustomerType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("CustomerType");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.District", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<double?>("Lat")
                        .HasColumnType("float");

                    b.Property<double?>("Lng")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Prefix")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("District");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.GroupPermission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CreatedByStaffId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("GroupPermission");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("PermissionGroupId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PermissionGroupId");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.PermissionGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("PermissionGroup");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Staff", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("FullName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid?>("GroupPermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(15)");

                    b.Property<Guid>("StaffTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StoreBranchId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StoreId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("GroupPermissionId");

                    b.HasIndex("StaffTypeId");

                    b.HasIndex("StoreBranchId");

                    b.HasIndex("StoreId");

                    b.ToTable("Staff");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.StaffType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EnumValue")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("StaffType");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.State", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("varchar(5)");

                    b.Property<string>("CountryCode")
                        .HasColumnType("varchar(5)");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("State");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Store", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BusinessAreaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<Guid>("InitialStoreAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("BusinessAreaId");

                    b.HasIndex("CurrencyId");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.StoreBranch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastSavedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastSavedUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StoreId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("StoreBranch");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Ward", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<double?>("Lat")
                        .HasColumnType("float");

                    b.Property<double?>("Lng")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Prefix")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Ward");
                });

            modelBuilder.Entity("GroupPermissionPermission", b =>
                {
                    b.Property<Guid>("GroupPermissionsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("PermissionsId")
                        .HasColumnType("int");

                    b.HasKey("GroupPermissionsId", "PermissionsId");

                    b.HasIndex("PermissionsId");

                    b.ToTable("GroupPermissionPermission");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Account", b =>
                {
                    b.HasOne("GoFoodBeverage.Domain.Entities.AccountType", "AccountType")
                        .WithMany("Accounts")
                        .HasForeignKey("AccountTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountType");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Address", b =>
                {
                    b.HasOne("GoFoodBeverage.Domain.Entities.City", "City")
                        .WithMany("Addresses")
                        .HasForeignKey("CityId");

                    b.HasOne("GoFoodBeverage.Domain.Entities.Country", "Country")
                        .WithMany("Addresses")
                        .HasForeignKey("CountryId");

                    b.HasOne("GoFoodBeverage.Domain.Entities.District", "District")
                        .WithMany("Addresses")
                        .HasForeignKey("DistrictId");

                    b.HasOne("GoFoodBeverage.Domain.Entities.State", "State")
                        .WithMany("Addresses")
                        .HasForeignKey("StateId");

                    b.HasOne("GoFoodBeverage.Domain.Entities.Ward", "Ward")
                        .WithMany("Addresses")
                        .HasForeignKey("WardId");

                    b.Navigation("City");

                    b.Navigation("Country");

                    b.Navigation("District");

                    b.Navigation("State");

                    b.Navigation("Ward");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Customer", b =>
                {
                    b.HasOne("GoFoodBeverage.Domain.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoFoodBeverage.Domain.Entities.CustomerType", "CustomerType")
                        .WithMany("Customers")
                        .HasForeignKey("CustomerTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoFoodBeverage.Domain.Entities.Store", "Store")
                        .WithMany("Customers")
                        .HasForeignKey("StoreId");

                    b.Navigation("Account");

                    b.Navigation("CustomerType");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Permission", b =>
                {
                    b.HasOne("GoFoodBeverage.Domain.Entities.PermissionGroup", "PermissionGroup")
                        .WithMany("Permissions")
                        .HasForeignKey("PermissionGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PermissionGroup");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Staff", b =>
                {
                    b.HasOne("GoFoodBeverage.Domain.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoFoodBeverage.Domain.Entities.GroupPermission", "GroupPermission")
                        .WithMany("Staffs")
                        .HasForeignKey("GroupPermissionId");

                    b.HasOne("GoFoodBeverage.Domain.Entities.StaffType", "StaffType")
                        .WithMany("Staffs")
                        .HasForeignKey("StaffTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoFoodBeverage.Domain.Entities.StoreBranch", "StoreBranch")
                        .WithMany("Staffs")
                        .HasForeignKey("StoreBranchId");

                    b.HasOne("GoFoodBeverage.Domain.Entities.Store", "Store")
                        .WithMany("Staffs")
                        .HasForeignKey("StoreId");

                    b.Navigation("Account");

                    b.Navigation("GroupPermission");

                    b.Navigation("StaffType");

                    b.Navigation("Store");

                    b.Navigation("StoreBranch");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Store", b =>
                {
                    b.HasOne("GoFoodBeverage.Domain.Entities.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoFoodBeverage.Domain.Entities.BusinessArea", "BusinessArea")
                        .WithMany("Stores")
                        .HasForeignKey("BusinessAreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoFoodBeverage.Domain.Entities.Currency", "Currency")
                        .WithMany("Stores")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("BusinessArea");

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.StoreBranch", b =>
                {
                    b.HasOne("GoFoodBeverage.Domain.Entities.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store");
                });

            modelBuilder.Entity("GroupPermissionPermission", b =>
                {
                    b.HasOne("GoFoodBeverage.Domain.Entities.GroupPermission", null)
                        .WithMany()
                        .HasForeignKey("GroupPermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoFoodBeverage.Domain.Entities.Permission", null)
                        .WithMany()
                        .HasForeignKey("PermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.AccountType", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.BusinessArea", b =>
                {
                    b.Navigation("Stores");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.City", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Country", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Currency", b =>
                {
                    b.Navigation("Stores");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.CustomerType", b =>
                {
                    b.Navigation("Customers");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.District", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.GroupPermission", b =>
                {
                    b.Navigation("Staffs");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.PermissionGroup", b =>
                {
                    b.Navigation("Permissions");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.StaffType", b =>
                {
                    b.Navigation("Staffs");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.State", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Store", b =>
                {
                    b.Navigation("Customers");

                    b.Navigation("Staffs");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.StoreBranch", b =>
                {
                    b.Navigation("Staffs");
                });

            modelBuilder.Entity("GoFoodBeverage.Domain.Entities.Ward", b =>
                {
                    b.Navigation("Addresses");
                });
#pragma warning restore 612, 618
        }
    }
}
