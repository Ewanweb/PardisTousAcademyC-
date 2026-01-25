using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarFileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroSlides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActionLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSlides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuccessStories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActionLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessStories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Client = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthLog_User",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeoMetadata_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoMetadata_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoMetadata_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoMetadata_NoIndex = table.Column<bool>(type: "bit", nullable: false),
                    SeoMetadata_NoFollow = table.Column<bool>(type: "bit", nullable: false),
                    UserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogCategories_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlogCategory_User",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalAmount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CoursesCount = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Seo_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_NoIndex = table.Column<bool>(type: "bit", nullable: false),
                    Seo_NoFollow = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalAmount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CartSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BlogCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SummaryDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeoMetadata_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoMetadata_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoMetadata_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoMetadata_NoIndex = table.Column<bool>(type: "bit", nullable: false),
                    SeoMetadata_NoFollow = table.Column<bool>(type: "bit", nullable: false),
                    Views = table.Column<long>(type: "bigint", nullable: false),
                    BlogCategoryId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_BlogCategory",
                        column: x => x.BlogCategoryId,
                        principalTable: "BlogCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_User",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_BlogCategories_BlogCategoryId1",
                        column: x => x.BlogCategoryId1,
                        principalTable: "BlogCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Schedule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsStarted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Seo_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seo_NoIndex = table.Column<bool>(type: "bit", nullable: false),
                    Seo_NoFollow = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiptImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptUploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitPrice = table.Column<long>(type: "bigint", nullable: false),
                    TitleSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThumbnailSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstructorSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdminNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseComments_AspNetUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseComments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalAmount = table.Column<long>(type: "bigint", nullable: false),
                    PaidAmount = table.Column<long>(type: "bigint", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    EnrollmentStatus = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsInstallmentAllowed = table.Column<bool>(type: "bit", nullable: false),
                    InstallmentCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    EnrolledCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSchedules_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSections", x => new { x.CourseId, x.Id });
                    table.ForeignKey(
                        name: "FK_CourseSections_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Gateway = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefundReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundAmount = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCourses",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasePrice = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttendedSessionsCount = table.Column<int>(type: "int", nullable: false),
                    AbsentSessionsCount = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExclusiveLiveLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAccessBlocked = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalGrade = table.Column<int>(type: "int", nullable: true),
                    CertificateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstructorNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourses", x => new { x.UserId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_UserCourses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstallmentPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstallmentNumber = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    PaidAmount = table.Column<long>(type: "bigint", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstallmentPayments_CourseEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "CourseEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    SessionNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSessions_CourseSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "CourseSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSessions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCourseSchedules",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttendedSessions = table.Column<int>(type: "int", nullable: false),
                    AbsentSessions = table.Column<int>(type: "int", nullable: false),
                    InstructorNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourseSchedules", x => new { x.UserId, x.CourseScheduleId });
                    table.ForeignKey(
                        name: "FK_UserCourseSchedules_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCourseSchedules_CourseSchedules_CourseScheduleId",
                        column: x => x.CourseScheduleId,
                        principalTable: "CourseSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_CourseSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "CourseSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "Avatar", "AvatarFileId", "AvatarUpdatedAt", "AvatarUrl", "Bio", "BirthDate", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "FullName", "Gender", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "Mobile", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "2c4e6097-f570-4927-b2f7-5f65d1373555", 0, null, null, null, null, null, null, null, "45c10d88-dffa-47f3-b155-c0f31768d894", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "sara@pardis.com", true, null, "سارا مدرس", null, true, null, false, null, null, "SARA@PARDIS.COM", "SARA@PARDIS.COM", "AQAAAAIAAYagAAAAEND7ESaocdvzJ2rGo7YsRAsv2HtmuXIu6DK7mDh2c0kTVKGQoWQUEoInjgjH/9I4gQ==", null, false, "2c4e6097-f570-4927-b2f7-5f65d1373555", false, "sara@pardis.com" },
                    { "8e445865-a24d-4543-a6c6-9443d048cdb9", 0, null, null, null, null, null, null, null, "900bdca3-1806-4699-bc69-60cd45e01f32", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "admin@pardis.com", true, null, "مدیر سیستم", null, true, null, false, null, null, "ADMIN@PARDIS.COM", "ADMIN@PARDIS.COM", "AQAAAAIAAYagAAAAEGVFu3McpAx5UMSLBJxugUKNBTPkqZGi82773srQiboES/oc+2wyxt+9T1sQnZtxMw==", null, false, "8e445865-a24d-4543-a6c6-9443d048cdb9", false, "09152003530" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "CreatedAt", "Description", "IsPublic", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "شماره کارت مقصد برای پرداخت دستی", true, "ManualPayment.CardNumber", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "6037-9977-****-****" },
                    { new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "نام صاحب کارت", true, "ManualPayment.CardHolder", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "آکادمی پردیس توس" },
                    { new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "نام بانک", true, "ManualPayment.BankName", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "بانک پاسارگاد" },
                    { new Guid("d4444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "توضیحات پرداخت دستی", true, "ManualPayment.Description", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "لطفاً پس از واریز، رسید پرداخت را آپلود کنید" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CoursesCount", "CreatedAt", "CreatedById", "Image", "IsActive", "ParentId", "Slug", "Title", "UpdatedAt", "Seo_CanonicalUrl", "Seo_MetaDescription", "Seo_MetaTitle", "Seo_NoFollow", "Seo_NoIndex" },
                values: new object[,]
                {
                    { new Guid("b1111111-1111-1111-1111-111111111111"), 3, new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "8e445865-a24d-4543-a6c6-9443d048cdb9", null, true, null, "programming", "برنامه نویسی", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "جامع ترین دوره ها", "آموزش برنامه نویسی", false, false },
                    { new Guid("b4444444-4444-4444-4444-444444444444"), 0, new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "2c4e6097-f570-4927-b2f7-5f65d1373555", null, true, null, "art-design", "هنر و طراحی", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "نقاشی و طراحی", "آموزش هنر", false, false },
                    { new Guid("b2222222-2222-2222-2222-222222222222"), 2, new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "8e445865-a24d-4543-a6c6-9443d048cdb9", null, true, new Guid("b1111111-1111-1111-1111-111111111111"), "web-development", "توسعه وب", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "ASP.NET و React", "آموزش طراحی سایت", false, false },
                    { new Guid("b3333333-3333-3333-3333-333333333333"), 1, new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "8e445865-a24d-4543-a6c6-9443d048cdb9", null, true, new Guid("b1111111-1111-1111-1111-111111111111"), "mobile-development", "برنامه نویسی موبایل", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "فلاتر و کاتلین", "آموزش موبایل", false, false }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "DeletedAt", "Description", "InstructorId", "IsCompleted", "IsDeleted", "IsStarted", "Location", "Price", "Schedule", "Slug", "StartFrom", "Status", "Thumbnail", "Title", "Type", "UpdatedAt", "Seo_CanonicalUrl", "Seo_MetaDescription", "Seo_MetaTitle", "Seo_NoFollow", "Seo_NoIndex" },
                values: new object[,]
                {
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("b2222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "آموزش پروژه محور", "8e445865-a24d-4543-a6c6-9443d048cdb9", true, false, true, "https://google.com", 2500000L, "Saturday 10:00", "aspnet-core-8-comprehensive", "Saturday 10:00", 1, null, "دوره جامع ASP.NET Core 8", 1, new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "بهترین دوره بک اند", "دوره ASP.NET Core", false, false },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), new Guid("b2222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "از مقدماتی تا پیشرفته", "2c4e6097-f570-4927-b2f7-5f65d1373555", false, false, true, "آموزشگاه", 1800000L, "Saturday 10:00", "react-nextjs-mastery", "Saturday 10:00", 1, null, "متخصص React و Next.js", 2, new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "آموزش فرانت اند", "دوره React", false, false },
                    { new Guid("c3333333-3333-3333-3333-333333333333"), new Guid("b3333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "ساخت اپلیکیشن", "8e445865-a24d-4543-a6c6-9443d048cdb9", false, false, false, "https://google.com", 3000000L, "Saturday 10:00", "flutter-zero-to-hero", "Saturday 10:00", 0, null, "فلاتر: از صفر تا صد", 1, new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "برنامه نویسی موبایل", "دوره Flutter", false, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuthLogs_UserId",
                table: "AuthLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_UserId",
                table: "BlogCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_UserId1",
                table: "BlogCategories",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_CourseId",
                table: "CartItems",
                columns: new[] { "CartId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CourseId",
                table: "CartItems",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseComments_CourseId",
                table: "CourseComments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseComments_ReviewedByUserId",
                table: "CourseComments",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseComments_UserId",
                table: "CourseComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_CourseId_StudentId",
                table: "CourseEnrollments",
                columns: new[] { "CourseId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_StudentId",
                table: "CourseEnrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSchedules_CourseId",
                table: "CourseSchedules",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_CourseId_SessionNumber",
                table: "CourseSessions",
                columns: new[] { "CourseId", "SessionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_ScheduleId",
                table: "CourseSessions",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentPayments_EnrollmentId_InstallmentNumber",
                table: "InstallmentPayments",
                columns: new[] { "EnrollmentId", "InstallmentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId_CartId_Active",
                table: "Orders",
                columns: new[] { "UserId", "CartId" },
                unique: true,
                filter: "[Status] IN (0, 1)");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_OrderId",
                table: "PaymentAttempts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_TrackingCode",
                table: "PaymentAttempts",
                column: "TrackingCode",
                unique: true,
                filter: "[TrackingCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_UserId",
                table: "PaymentAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BlogCategoryId",
                table: "Posts",
                column: "BlogCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BlogCategoryId1",
                table: "Posts",
                column: "BlogCategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId1",
                table: "Posts",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_RecordedByUserId",
                table: "StudentAttendances",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_SessionId_StudentId",
                table: "StudentAttendances",
                columns: new[] { "SessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudentId",
                table: "StudentAttendances",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_Level_Time",
                table: "SystemLogs",
                columns: new[] { "Level", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_Source_Time",
                table: "SystemLogs",
                columns: new[] { "Source", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_Time",
                table: "SystemLogs",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CourseId",
                table: "Transactions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionId",
                table: "Transactions",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourses_CourseId",
                table: "UserCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseSchedules_CourseScheduleId",
                table: "UserCourseSchedules",
                column: "CourseScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuthLogs");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "CourseComments");

            migrationBuilder.DropTable(
                name: "CourseSections");

            migrationBuilder.DropTable(
                name: "HeroSlides");

            migrationBuilder.DropTable(
                name: "InstallmentPayments");

            migrationBuilder.DropTable(
                name: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "PaymentSettings");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "StudentAttendances");

            migrationBuilder.DropTable(
                name: "SuccessStories");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserCourses");

            migrationBuilder.DropTable(
                name: "UserCourseSchedules");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "CourseEnrollments");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropTable(
                name: "CourseSessions");

            migrationBuilder.DropTable(
                name: "CourseSchedules");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
