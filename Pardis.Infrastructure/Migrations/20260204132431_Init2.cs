using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_MetaTitle",
                table: "Posts",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_MetaDescription",
                table: "Posts",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_CanonicalUrl",
                table: "Posts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_JsonLdSchemas",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_Keywords",
                table: "Posts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_OpenGraphDescription",
                table: "Posts",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_OpenGraphImage",
                table: "Posts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_OpenGraphTitle",
                table: "Posts",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_OpenGraphType",
                table: "Posts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_TwitterCardType",
                table: "Posts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_TwitterDescription",
                table: "Posts",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_TwitterImage",
                table: "Posts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_TwitterTitle",
                table: "Posts",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "PaymentAttempts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PaymentAttempts",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<bool>(
                name: "Seo_NoIndex",
                table: "Courses",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Seo_NoFollow",
                table: "Courses",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Seo_MetaTitle",
                table: "Courses",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_MetaDescription",
                table: "Courses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_CanonicalUrl",
                table: "Courses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_JsonLdSchemas",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_Keywords",
                table: "Courses",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OpenGraphDescription",
                table: "Courses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OpenGraphImage",
                table: "Courses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OpenGraphTitle",
                table: "Courses",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OpenGraphType",
                table: "Courses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterCardType",
                table: "Courses",
                type: "nvarchar(20)",
                maxLength: 20,
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
                name: "Seo_TwitterTitle",
                table: "Courses",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Seo_NoIndex",
                table: "Categories",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Seo_NoFollow",
                table: "Categories",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Seo_MetaTitle",
                table: "Categories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_MetaDescription",
                table: "Categories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_CanonicalUrl",
                table: "Categories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_JsonLdSchemas",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_Keywords",
                table: "Categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OpenGraphDescription",
                table: "Categories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OpenGraphImage",
                table: "Categories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OpenGraphTitle",
                table: "Categories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_OpenGraphType",
                table: "Categories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seo_TwitterCardType",
                table: "Categories",
                type: "nvarchar(20)",
                maxLength: 20,
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
                name: "Seo_TwitterTitle",
                table: "Categories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_MetaTitle",
                table: "BlogCategories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_MetaDescription",
                table: "BlogCategories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_CanonicalUrl",
                table: "BlogCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_JsonLdSchemas",
                table: "BlogCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_Keywords",
                table: "BlogCategories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_OpenGraphDescription",
                table: "BlogCategories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_OpenGraphImage",
                table: "BlogCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_OpenGraphTitle",
                table: "BlogCategories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_OpenGraphType",
                table: "BlogCategories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_TwitterCardType",
                table: "BlogCategories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_TwitterDescription",
                table: "BlogCategories",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_TwitterImage",
                table: "BlogCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoMetadata_TwitterTitle",
                table: "BlogCategories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAuditLogs_AspNetUsers_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentAuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0aa101fa-c85d-4e71-ab3c-3f19055e411f", "AQAAAAIAAYagAAAAEM8RfCvBgzVQ/YlkWpwqBuFp2NyGxOVf4q1V0m4Nlp0H28DVAILy2O+I/blV7gdhmQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f95a30cf-3210-4c5b-bd99-b90364f039bc", "AQAAAAIAAYagAAAAEKxLR1AH1r6YQ/5dmWkIN10F/K6Z9kwKh+SvuEr9lA+B9k7j0368BD/iy5KPdGdx8Q==" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"),
                columns: new[] { "Description", "Seo_JsonLdSchemas", "Seo_Keywords", "Seo_OpenGraphDescription", "Seo_OpenGraphImage", "Seo_OpenGraphTitle", "Seo_OpenGraphType", "Seo_TwitterCardType", "Seo_TwitterDescription", "Seo_TwitterImage", "Seo_TwitterTitle" },
                values: new object[] { null, null, null, null, null, null, "website", "summary_large_image", null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                columns: new[] { "Description", "Seo_JsonLdSchemas", "Seo_Keywords", "Seo_OpenGraphDescription", "Seo_OpenGraphImage", "Seo_OpenGraphTitle", "Seo_OpenGraphType", "Seo_TwitterCardType", "Seo_TwitterDescription", "Seo_TwitterImage", "Seo_TwitterTitle" },
                values: new object[] { null, null, null, null, null, null, "website", "summary_large_image", null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b3333333-3333-3333-3333-333333333333"),
                columns: new[] { "Description", "Seo_JsonLdSchemas", "Seo_Keywords", "Seo_OpenGraphDescription", "Seo_OpenGraphImage", "Seo_OpenGraphTitle", "Seo_OpenGraphType", "Seo_TwitterCardType", "Seo_TwitterDescription", "Seo_TwitterImage", "Seo_TwitterTitle" },
                values: new object[] { null, null, null, null, null, null, "website", "summary_large_image", null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b4444444-4444-4444-4444-444444444444"),
                columns: new[] { "Description", "Seo_JsonLdSchemas", "Seo_Keywords", "Seo_OpenGraphDescription", "Seo_OpenGraphImage", "Seo_OpenGraphTitle", "Seo_OpenGraphType", "Seo_TwitterCardType", "Seo_TwitterDescription", "Seo_TwitterImage", "Seo_TwitterTitle" },
                values: new object[] { null, null, null, null, null, null, "website", "summary_large_image", null, null, null });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c1111111-1111-1111-1111-111111111111"),
                columns: new[] { "Seo_JsonLdSchemas", "Seo_Keywords", "Seo_OpenGraphDescription", "Seo_OpenGraphImage", "Seo_OpenGraphTitle", "Seo_OpenGraphType", "Seo_TwitterCardType", "Seo_TwitterDescription", "Seo_TwitterImage", "Seo_TwitterTitle" },
                values: new object[] { null, null, null, null, null, "website", "summary_large_image", null, null, null });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c2222222-2222-2222-2222-222222222222"),
                columns: new[] { "Seo_JsonLdSchemas", "Seo_Keywords", "Seo_OpenGraphDescription", "Seo_OpenGraphImage", "Seo_OpenGraphTitle", "Seo_OpenGraphType", "Seo_TwitterCardType", "Seo_TwitterDescription", "Seo_TwitterImage", "Seo_TwitterTitle" },
                values: new object[] { null, null, null, null, null, "website", "summary_large_image", null, null, null });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                columns: new[] { "Seo_JsonLdSchemas", "Seo_Keywords", "Seo_OpenGraphDescription", "Seo_OpenGraphImage", "Seo_OpenGraphTitle", "Seo_OpenGraphType", "Seo_TwitterCardType", "Seo_TwitterDescription", "Seo_TwitterImage", "Seo_TwitterTitle" },
                values: new object[] { null, null, null, null, null, "website", "summary_large_image", null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAuditLogs_AdminUserId",
                table: "PaymentAuditLogs",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAuditLogs_UserId",
                table: "PaymentAuditLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentAuditLogs");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_JsonLdSchemas",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_Keywords",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_OpenGraphDescription",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_OpenGraphImage",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_OpenGraphTitle",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_OpenGraphType",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_TwitterCardType",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_TwitterDescription",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_TwitterImage",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_TwitterTitle",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "Seo_JsonLdSchemas",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_Keywords",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_OpenGraphDescription",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_OpenGraphImage",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_OpenGraphTitle",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_OpenGraphType",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_TwitterCardType",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_TwitterDescription",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_TwitterImage",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Seo_TwitterTitle",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_JsonLdSchemas",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_Keywords",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_OpenGraphDescription",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_OpenGraphImage",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_OpenGraphTitle",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_OpenGraphType",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_TwitterCardType",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_TwitterDescription",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_TwitterImage",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Seo_TwitterTitle",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_JsonLdSchemas",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_Keywords",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_OpenGraphDescription",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_OpenGraphImage",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_OpenGraphTitle",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_OpenGraphType",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_TwitterCardType",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_TwitterDescription",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_TwitterImage",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "SeoMetadata_TwitterTitle",
                table: "BlogCategories");

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_MetaTitle",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_MetaDescription",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_CanonicalUrl",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Seo_NoIndex",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Seo_NoFollow",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_MetaTitle",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_MetaDescription",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_CanonicalUrl",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Seo_NoIndex",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Seo_NoFollow",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_MetaTitle",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_MetaDescription",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Seo_CanonicalUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_MetaTitle",
                table: "BlogCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_MetaDescription",
                table: "BlogCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoMetadata_CanonicalUrl",
                table: "BlogCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4cc3ea46-c871-4e7b-9d9f-d688f4404b0c", "AQAAAAIAAYagAAAAEE8oQmcRLG7MdkZT0q3p/eaX4vEhWPh2CY3JuUrUaiRsuahEvKnaituuaCIghCHU7A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3aea6584-9ed6-4ad2-b3a6-3f774f1f7e40", "AQAAAAIAAYagAAAAEOKMSsvTIDwXHuR6Yo6VfMtJjUgoWRRsDVqbEuloQKDqPIfxx4NpwH0TwkuM3brCPg==" });
        }
    }
}
