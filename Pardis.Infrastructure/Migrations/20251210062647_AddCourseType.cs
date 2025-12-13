using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1c947764-ca65-4ca0-9db0-97ca4c00a47b", "AQAAAAIAAYagAAAAEF8lsOS4PRslvNtQA7ux1ut920o6TF/NySZvv8lHcHcQsGJlycWrwvct8Z0eR19oqg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1f8c3aab-3e64-42a2-b0e1-313c0d5c453e", "AQAAAAIAAYagAAAAEFfo3oUTj8Kqlt+VDtNE3f/BQiUr99Jy6wXQQitlYj4sM2GiV22LObjksEDgAlge8Q==" });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c1111111-1111-1111-1111-111111111111"),
                columns: new[] { "Location", "Type" },
                values: new object[] { "https://google.com", 2 });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c2222222-2222-2222-2222-222222222222"),
                columns: new[] { "Location", "Type" },
                values: new object[] { "آموزشگاه", 3 });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                columns: new[] { "Location", "Type" },
                values: new object[] { "https://google.com", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Courses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4bb940ac-c60d-4772-9acc-b0ee8c3e7bcf", "AQAAAAIAAYagAAAAEPws3GMAamoUEppk4pV5e0QiVJNDo7qtNMyCV7yeJ17xznhNy0OjR28sdWZCC5Z84A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ccc2ce25-e130-4c66-8247-80b7da6c0c6a", "AQAAAAIAAYagAAAAEBIb5SF0spI316EgOHzFLyRbhActWD+qN6MKNdvj6f4+hMwGbJPNNqJUctbp8od7NQ==" });
        }
    }
}
