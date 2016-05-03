using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Stolons.Models;

namespace Stolons.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160501160624_Migrations")]
    partial class Migrations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("Stolons.Models.ApplicationConfig", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsModeSimulated");

                    b.Property<int>("OrderDayStartDate");

                    b.Property<string>("OrderDeliveryMessage");

                    b.Property<int>("OrderHourStartDate");

                    b.Property<int>("OrderMinuteStartDate");

                    b.Property<int>("PreparationDayStartDate");

                    b.Property<int>("PreparationHourStartDate");

                    b.Property<int>("PreparationMinuteStartDate");

                    b.Property<int>("SimulationMode");

                    b.Property<int>("StockUpdateDayStartDate");

                    b.Property<int>("StockUpdateHourStartDate");

                    b.Property<int>("StockUpdateMinuteStartDate");

                    b.Property<string>("StolonsMailAdress");

                    b.Property<string>("StolonsMailPassword");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Stolons.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<int?>("UserId");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("Stolons.Models.Bill", b =>
                {
                    b.Property<string>("BillNumber");

                    b.Property<int?>("ConsumerId");

                    b.Property<int>("State");

                    b.Property<int?>("UserId");

                    b.HasKey("BillNumber");
                });

            modelBuilder.Entity("Stolons.Models.BillEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<Guid?>("TempWeekBasketId");

                    b.Property<Guid?>("ValidatedWeekBasketId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Stolons.Models.News", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateOfPublication");

                    b.Property<string>("ImageLink");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("UserForeignKey");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Stolons.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AverageQuantity");

                    b.Property<DateTime>("DLC");

                    b.Property<string>("Description");

                    b.Property<string>("FamillyFamillyName");

                    b.Property<string>("LabelsSerialized");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PicturesSerialized");

                    b.Property<float>("Price");

                    b.Property<int?>("ProducerId");

                    b.Property<int>("ProductUnit");

                    b.Property<int>("QuantityStep");

                    b.Property<int>("RemainingStock");

                    b.Property<int>("State");

                    b.Property<int>("Type");

                    b.Property<int>("WeekStock");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Stolons.Models.ProductFamilly", b =>
                {
                    b.Property<string>("FamillyName");

                    b.Property<string>("Image");

                    b.Property<string>("TypeName");

                    b.HasKey("FamillyName");
                });

            modelBuilder.Entity("Stolons.Models.ProductType", b =>
                {
                    b.Property<string>("Name");

                    b.Property<string>("Image");

                    b.HasKey("Name");
                });

            modelBuilder.Entity("Stolons.Models.TempWeekBasket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ConsumerId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Stolons.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Avatar");

                    b.Property<string>("City");

                    b.Property<bool>("Cotisation");

                    b.Property<string>("DisableReason");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<bool>("Enable");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("PostCode");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<string>("Surname")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "User");
                });

            modelBuilder.Entity("Stolons.Models.ValidatedWeekBasket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ConsumerId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Stolons.Models.Consumer", b =>
                {
                    b.HasBaseType("Stolons.Models.User");


                    b.HasAnnotation("Relational:DiscriminatorValue", "Consumer");
                });

            modelBuilder.Entity("Stolons.Models.Producer", b =>
                {
                    b.HasBaseType("Stolons.Models.User");

                    b.Property<int>("Area");

                    b.Property<string>("CompanyName");

                    b.Property<string>("ExploitationPicuresSerialized");

                    b.Property<string>("OpenText");

                    b.Property<string>("Production");

                    b.Property<int>("StartDate");

                    b.Property<string>("WebSiteLink");

                    b.HasAnnotation("Relational:DiscriminatorValue", "Producer");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Stolons.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Stolons.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("Stolons.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Stolons.Models.ApplicationUser", b =>
                {
                    b.HasOne("Stolons.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Stolons.Models.Bill", b =>
                {
                    b.HasOne("Stolons.Models.Consumer")
                        .WithMany()
                        .HasForeignKey("ConsumerId");

                    b.HasOne("Stolons.Models.Producer")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Stolons.Models.BillEntry", b =>
                {
                    b.HasOne("Stolons.Models.Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("Stolons.Models.TempWeekBasket")
                        .WithMany()
                        .HasForeignKey("TempWeekBasketId");

                    b.HasOne("Stolons.Models.ValidatedWeekBasket")
                        .WithMany()
                        .HasForeignKey("ValidatedWeekBasketId");
                });

            modelBuilder.Entity("Stolons.Models.News", b =>
                {
                    b.HasOne("Stolons.Models.User")
                        .WithMany()
                        .HasForeignKey("UserForeignKey");
                });

            modelBuilder.Entity("Stolons.Models.Product", b =>
                {
                    b.HasOne("Stolons.Models.ProductFamilly")
                        .WithMany()
                        .HasForeignKey("FamillyFamillyName");

                    b.HasOne("Stolons.Models.Producer")
                        .WithMany()
                        .HasForeignKey("ProducerId");
                });

            modelBuilder.Entity("Stolons.Models.ProductFamilly", b =>
                {
                    b.HasOne("Stolons.Models.ProductType")
                        .WithMany()
                        .HasForeignKey("TypeName");
                });

            modelBuilder.Entity("Stolons.Models.TempWeekBasket", b =>
                {
                    b.HasOne("Stolons.Models.Consumer")
                        .WithOne()
                        .HasForeignKey("Stolons.Models.TempWeekBasket", "ConsumerId");
                });

            modelBuilder.Entity("Stolons.Models.ValidatedWeekBasket", b =>
                {
                    b.HasOne("Stolons.Models.Consumer")
                        .WithMany()
                        .HasForeignKey("ConsumerId");
                });
        }
    }
}