using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2047ec6a-d4fd-45a7-b79f-cb2280cd0d49", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEF5TqlDTc1npHgvSd+pfdj+PVWLeGuBQtizpeNhCovVmc1Fr7TaSjA3odpbBiXlySA==", "2c4e6097-f570-4927-b2f7-5f65d1373555" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "184d931e-c3cf-44a0-b82a-f62de68aa704", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEIkeXzsvJAPVVhf7ChNU6Sg51zC02CRxzfDUjAdzesAqpk/eL32wpGHhhPfCAxT/iA==", "8e445865-a24d-4543-a6c6-9443d048cdb9" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b3333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b4444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c1111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "آموزش پروژه محور", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c2222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "از مقدماتی تا پیشرفته", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Description", "Title", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "ساخت اپلیکیشن", "فلاتر: از صفر تا صد", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e9b16540-ee16-4615-9f7a-f9e4e71dbe26", new DateTime(2025, 11, 27, 14, 59, 37, 568, DateTimeKind.Utc).AddTicks(204), "AQAAAAIAAYagAAAAEG/MT37D3fC8LSwZV5reS/YUVPRcQQzjzVWEhwez09HMLpJdIv8xTltI92FWP73gsg==", "e3e55c43-1464-4f40-8f88-6b096c2ad4d9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "df887a21-02c9-4f5f-81bd-61d375ef37ee", new DateTime(2025, 11, 27, 14, 59, 37, 521, DateTimeKind.Utc).AddTicks(9967), "AQAAAAIAAYagAAAAED40izAXJyj5r2zn0R52ivfGCmQ4pQehST09P5vplvA8Z6da+4d9Upc4FKE3+yUxGA==", "610e0848-0cb6-4081-8b24-80ddb8aca59e" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9028), new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9030) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9964), new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9965) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b3333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9971), new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9971) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b4444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9974), new DateTime(2025, 11, 27, 14, 59, 37, 608, DateTimeKind.Utc).AddTicks(9975) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c1111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(2747), "آموزش پروژه محور ساخت سایت فروشگاهی با معماری Clean", new DateTime(2025, 11, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(2848) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c2222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Description", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(3417), "از مقدماتی تا پیشرفته همراه با هوک ها", new DateTime(2025, 11, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(3421) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Description", "Title", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(3425), "ساخت اپلیکیشن های اندروید و iOS با یک کد", "فلاتر: از صفر تا انتشار در مارکت", new DateTime(2025, 11, 27, 14, 59, 37, 609, DateTimeKind.Utc).AddTicks(3425) });
        }
    }
}
