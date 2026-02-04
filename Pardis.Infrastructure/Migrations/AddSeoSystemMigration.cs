using Microsoft.EntityFrameworkCore.Migrations;
using Pardis.Domain.Seo;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeoSystemMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add SEO columns to Categories table
            migrationBuilder.AddColumn<string>(
                name: "Seo_Title",
                table: "Categories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_Description",
                table: "Categories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_Keywords",
                table: "Categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_CanonicalUrl",
                table: "Categories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Seo_NoIndex",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Seo_NoFollow",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OgTitle",
                table: "Categories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OgDescription",
                table: "Categories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OgImage",
                table: "Categories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OgType",
                table: "Categories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "website");

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterTitle",
                table: "Categories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterDescription",
                table: "Categories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterImage",
                table: "Categories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterCard",
                table: "Categories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "summary_large_image");

            migrationBuilder.AddColumn<string>(
                name: "Seo_JsonLdData",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            // Add SEO columns to Courses table
            migrationBuilder.AddColumn<string>(
                name: "Seo_Title",
                table: "Courses",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_Description",
                table: "Courses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_Keywords",
                table: "Courses",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_CanonicalUrl",
                table: "Courses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Seo_NoIndex",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Seo_NoFollow",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OgTitle",
                table: "Courses",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OgDescription",
                table: "Courses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OgImage",
                table: "Courses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OgType",
                table: "Courses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "article");

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterTitle",
                table: "Courses",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterDescription",
                table: "Courses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterImage",
                table: "Courses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterCard",
                table: "Courses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "summary_large_image");

            migrationBuilder.AddColumn<string>(
                name: "Seo_JsonLdData",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            // Create Pages table
            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Static"),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Language = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, defaultValue: "fa"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Seo_Title = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Seo_Description = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Seo_Keywords = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Seo_CanonicalUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Seo_NoIndex = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Seo_NoFollow = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Seo_OgTitle = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Seo_OgDescription = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Seo_OgImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Seo_OgType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "website"),
                    Seo_TwitterTitle = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Seo_TwitterDescription = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Seo_TwitterImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Seo_TwitterCard = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "summary_large_image"),
                    Seo_JsonLdData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                });

            // Create SlugRedirects table
            migrationBuilder.CreateTable(
                name: "SlugRedirects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldSlug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NewSlug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlugRedirects", x => x.Id);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Slug",
                table: "Courses",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_Slug",
                table: "Pages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageType",
                table: "Pages",
                column: "PageType");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_IsPublished",
                table: "Pages",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_Language",
                table: "Pages",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_SlugRedirects_OldSlug_EntityType",
                table: "SlugRedirects",
                columns: new[] { "OldSlug", "EntityType" });

            migrationBuilder.CreateIndex(
                name: "IX_SlugRedirects_IsActive",
                table: "SlugRedirects",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop tables
            migrationBuilder.DropTable(name: "Pages");
            migrationBuilder.DropTable(name: "SlugRedirects");

            // Drop indexes
            migrationBuilder.DropIndex(name: "IX_Categories_Slug", table: "Categories");
            migrationBuilder.DropIndex(name: "IX_Courses_Slug", table: "Courses");

            // Remove SEO columns from Categories
            migrationBuilder.DropColumn(name: "Seo_Title", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_Description", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_Keywords", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_CanonicalUrl", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_NoIndex", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_NoFollow", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_OgTitle", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_OgDescription", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_OgImage", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_OgType", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_TwitterTitle", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_TwitterDescription", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_TwitterImage", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_TwitterCard", table: "Categories");
            migrationBuilder.DropColumn(name: "Seo_JsonLdData", table: "Categories");

            // Remove SEO columns from Courses
            migrationBuilder.DropColumn(name: "Seo_Title", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_Description", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_Keywords", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_CanonicalUrl", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_NoIndex", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_NoFollow", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_OgTitle", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_OgDescription", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_OgImage", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_OgType", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_TwitterTitle", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_TwitterDescription", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_TwitterImage", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_TwitterCard", table: "Courses");
            migrationBuilder.DropColumn(name: "Seo_JsonLdData", table: "Courses");
        }
    }
}