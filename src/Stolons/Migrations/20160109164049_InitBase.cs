using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Stolons.Migrations
{
    public partial class InitBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Down(migrationBuilder);
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Speaker",
                columns: table => new
                {
                    SpeakerId = table.Column<Guid>(nullable: false),
                    Bio = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speaker", x => x.SpeakerId);
                });
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Cotisation = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Enable = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<bool>(nullable: false),
                    Surname = table.Column<string>(nullable: false),
                    UserRole = table.Column<int>(nullable: false),
                    Area = table.Column<int>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    ExploitationPicuresSerialized = table.Column<string>(nullable: true),
                    OpenText = table.Column<string>(nullable: true),
                    Production = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    WebSiteLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "ProductFamilly",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFamilly", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFamilly_ProductType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    TopicId = table.Column<Guid>(nullable: false),
                    SpeakerId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.TopicId);
                    table.ForeignKey(
                        name: "FK_Topic_Speaker_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "Speaker",
                        principalColumn: "SpeakerId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    BillNumber = table.Column<string>(nullable: false),
                    ConsumerId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.BillNumber);
                    table.ForeignKey(
                        name: "FK_Bill_Consumer_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bill_Producer_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "WeekBasket",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConsumerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekBasket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekBasket_Consumer_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AverageQuantity = table.Column<int>(nullable: false),
                    DLC = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FamillyId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PicturesSerialized = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    ProducerId = table.Column<int>(nullable: true),
                    ProductUnit = table.Column<int>(nullable: false),
                    QuantityStep = table.Column<int>(nullable: false),
                    RemainingStock = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    WeekStock = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductFamilly_FamillyId",
                        column: x => x.FamillyId,
                        principalTable: "ProductFamilly",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Producer_ProducerId",
                        column: x => x.ProducerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<string>", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "BillEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Validate = table.Column<bool>(nullable: false),
                    WeekBasketId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillEntry_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillEntry_WeekBasket_WeekBasketId",
                        column: x => x.WeekBasketId,
                        principalTable: "WeekBasket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Label",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Label", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Label_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");
            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("AspNetRoleClaims");
            migrationBuilder.DropTable("AspNetUserClaims");
            migrationBuilder.DropTable("AspNetUserLogins");
            migrationBuilder.DropTable("AspNetUserRoles");
            migrationBuilder.DropTable("Bill");
            migrationBuilder.DropTable("BillEntry");
            migrationBuilder.DropTable("Label");
            migrationBuilder.DropTable("Topic");
            migrationBuilder.DropTable("AspNetRoles");
            migrationBuilder.DropTable("AspNetUsers");
            migrationBuilder.DropTable("WeekBasket");
            migrationBuilder.DropTable("Product");
            migrationBuilder.DropTable("Speaker");
            migrationBuilder.DropTable("ProductFamilly");
            migrationBuilder.DropTable("User");
            migrationBuilder.DropTable("ProductType");
        }
    }
}
