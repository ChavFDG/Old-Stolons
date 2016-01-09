using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Stolons.Models;

namespace Stolons.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160109134042_InitBase")]
    partial class InitBase
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

            modelBuilder.Entity("Test_ASP_MVC.Models.ApplicationUser", b =>
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

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Bill", b =>
                {
                    b.Property<string>("BillNumber");

                    b.Property<int?>("ConsumerId");

                    b.Property<int>("State");

                    b.Property<int?>("UserId");

                    b.HasKey("BillNumber");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.BillEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<bool>("Validate");

                    b.Property<Guid?>("WeekBasketId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.Property<Guid?>("ProductId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AverageQuantity");

                    b.Property<DateTime>("DLC");

                    b.Property<string>("Description");

                    b.Property<Guid?>("FamillyId");

                    b.Property<string>("Name");

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

            modelBuilder.Entity("Test_ASP_MVC.Models.ProductFamilly", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<Guid?>("TypeId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.ProductType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Speaker", b =>
                {
                    b.Property<Guid>("SpeakerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Bio");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("SpeakerId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Topic", b =>
                {
                    b.Property<Guid>("TopicId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("SpeakerId");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("TopicId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Avatar");

                    b.Property<string>("City");

                    b.Property<bool>("Cotisation");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<bool>("Enable");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("PostCode");

                    b.Property<bool>("RegistrationDate");

                    b.Property<string>("Surname")
                        .IsRequired();

                    b.Property<int>("UserRole");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "User");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.WeekBasket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ConsumerId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Consumer", b =>
                {
                    b.HasBaseType("Test_ASP_MVC.Models.User");


                    b.HasAnnotation("Relational:DiscriminatorValue", "Consumer");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Producer", b =>
                {
                    b.HasBaseType("Test_ASP_MVC.Models.User");

                    b.Property<int>("Area");

                    b.Property<string>("CompanyName");

                    b.Property<string>("ExploitationPicuresSerialized");

                    b.Property<string>("OpenText");

                    b.Property<string>("Production");

                    b.Property<DateTime>("StartDate");

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
                    b.HasOne("Test_ASP_MVC.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Test_ASP_MVC.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("Test_ASP_MVC.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Bill", b =>
                {
                    b.HasOne("Test_ASP_MVC.Models.Consumer")
                        .WithMany()
                        .HasForeignKey("ConsumerId");

                    b.HasOne("Test_ASP_MVC.Models.Producer")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.BillEntry", b =>
                {
                    b.HasOne("Test_ASP_MVC.Models.Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("Test_ASP_MVC.Models.WeekBasket")
                        .WithMany()
                        .HasForeignKey("WeekBasketId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Label", b =>
                {
                    b.HasOne("Test_ASP_MVC.Models.Product")
                        .WithMany()
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Product", b =>
                {
                    b.HasOne("Test_ASP_MVC.Models.ProductFamilly")
                        .WithMany()
                        .HasForeignKey("FamillyId");

                    b.HasOne("Test_ASP_MVC.Models.Producer")
                        .WithMany()
                        .HasForeignKey("ProducerId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.ProductFamilly", b =>
                {
                    b.HasOne("Test_ASP_MVC.Models.ProductType")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.Topic", b =>
                {
                    b.HasOne("Test_ASP_MVC.Models.Speaker")
                        .WithMany()
                        .HasForeignKey("SpeakerId");
                });

            modelBuilder.Entity("Test_ASP_MVC.Models.WeekBasket", b =>
                {
                    b.HasOne("Test_ASP_MVC.Models.Consumer")
                        .WithOne()
                        .HasForeignKey("Test_ASP_MVC.Models.WeekBasket", "ConsumerId");
                });
        }
    }
}
