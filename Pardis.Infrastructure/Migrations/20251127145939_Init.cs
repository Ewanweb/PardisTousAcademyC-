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
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FullName", "IsActive", "LockoutEnabled", "LockoutEnd", "Mobile", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "2c4e6097-f570-4927-b2f7-5f65d1373555", 0, null, "e9b16540-ee16-4615-9f7a-f9e4e71dbe26", new DateTime(2025, 11, 27, 14, 59, 37, 568, DateTimeKind.Utc).AddTicks(204), "sara@pardis.com", true, "سارا مدرس", true, false, null, null, "SARA@PARDIS.COM", "SARA@PARDIS.COM", "AQAAAAIAAYagAAAAEG/MT37D3fC8LSwZV5reS/YUVPRcQQzjzVWEhwez09HMLpJdIv8xTltI92FWP73gsg==", null, false, "e3e55c43-1464-4f40-8f88-6b096c2ad4d9", false, "sara@pardis.com" },
                    { "8e445865-a24d-4543-a6c6-9443d048cdb9", 0, null, "df887a21-02c9-4f5f-81bd-61d375ef37ee", new DateTime(2025, 11, 27, 14, 59, 37, 521, DateTimeKind.Utc).AddTicks(9967), "admin@pardis.com", true, "مدیر سیستم", true, false, null, null, "ADMIN@PARDIS.COM", "ADMIN@PARDIS.COM", "AQAAAAIAAYagAAAAED40izAXJyj5r2zn0R52ivfGCmQ4pQehST09P5vplvA8Z6da+4d9Upc4FKE3+yUxGA==", null, false, "610e0848-0cb6-4081-8b24-80ddb8aca59e", false, "admin@pardis.com" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CoursesCount", "CreatedAt", "CreatedById", "Image", "IsActive", "ParentId", "Slug", "Title", "UpdatedAt", "Seo_CanonicalUrl", "Seo_MetaDescription", "Seo_MetaTitle", "Seo_NoFollow", "Seo_NoIndex" },
                values: new object[,]
                {
                    { new Guid("b1111111-1111-1111-1111-111111111111"), 3, new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9028), "8e445865-a24d-4543-a6c6-9443d048cdb9", null, true, null, "programming", "برنامه نویسی", new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9030), null, "جامع ترین دوره ها", "آموزش برنامه نویسی", false, false },
                    { new Guid("b4444444-4444-4444-4444-444444444444"), 0, new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9974), "2c4e6097-f570-4927-b2f7-5f65d1373555", null, true, null, "art-design", "هنر و طراحی", new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9975), null, "نقاشی و طراحی", "آموزش هنر", false, false },
                    { new Guid("b2222222-2222-2222-2222-222222222222"), 2, new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9964), "8e445865-a24d-4543-a6c6-9443d048cdb9", null, true, new Guid("b1111111-1111-1111-1111-111111111111"), "web-development", "توسعه وب", new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9965), null, "ASP.NET و React", "آموزش طراحی سایت", false, false },
                    { new Guid("b3333333-3333-3333-3333-333333333333"), 1, new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9971), "8e445865-a24d-4543-a6c6-9443d048cdb9", null, true, new Guid("b1111111-1111-1111-1111-111111111111"), "mobile-development", "برنامه نویسی موبایل", new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9971), null, "فلاتر و کاتلین", "آموزش موبایل", false, false }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "DeletedAt", "Description", "InstructorId", "IsDeleted", "Price", "Slug", "Status", "Thumbnail", "Title", "UpdatedAt", "Seo_CanonicalUrl", "Seo_MetaDescription", "Seo_MetaTitle", "Seo_NoFollow", "Seo_NoIndex" },
                values: new object[,]
                {
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("b2222222-2222-2222-2222-222222222222"), new DateTime(2025, 9, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(2747), null, "آموزش پروژه محور ساخت سایت فروشگاهی با معماری Clean", "8e445865-a24d-4543-a6c6-9443d048cdb9", false, 2500000L, "aspnet-core-8-comprehensive", 1, null, "دوره جامع ASP.NET Core 8", new DateTime(2025, 11, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(2848), null, "بهترین دوره بک اند", "دوره ASP.NET Core", false, false },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), new Guid("b2222222-2222-2222-2222-222222222222"), new DateTime(2025, 10, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(3417), null, "از مقدماتی تا پیشرفته همراه با هوک ها", "2c4e6097-f570-4927-b2f7-5f65d1373555", false, 1800000L, "react-nextjs-mastery", 1, null, "متخصص React و Next.js", new DateTime(2025, 11, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(3421), null, "آموزش فرانت اند", "دوره React", false, false },
                    { new Guid("c3333333-3333-3333-3333-333333333333"), new Guid("b3333333-3333-3333-3333-333333333333"), new DateTime(2025, 11, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(3425), null, "ساخت اپلیکیشن های اندروید و iOS با یک کد", "8e445865-a24d-4543-a6c6-9443d048cdb9", false, 3000000L, "flutter-zero-to-hero", 0, null, "فلاتر: از صفر تا انتشار در مارکت", new DateTime(2025, 11, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(3425), null, "برنامه نویسی موبایل", "دوره Flutter", false, true }
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
                name: "IX_Categories_CreatedById",
                table: "Categories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses",
                column: "InstructorId");
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
                name: "Courses");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
