using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartFrom",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "66d44ad0-e36f-4d90-99a6-8e652b19716f", "AQAAAAIAAYagAAAAEI2jY5kaRj64hsFxa5sH9R8eP3x6fEwxcUA1rE7qbHaZnWEqb5aZpQ2NGQt63RtPLA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3c202414-695c-4f9a-8607-a71583ffa870", "AQAAAAIAAYagAAAAEEc7Wij07zscgr3ssaRlnJnAh47hA8dIIRb66SOvGESovaG63hTmD5qmVzvcvHGyAQ==" });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c1111111-1111-1111-1111-111111111111"),
                columns: new[] { "Schedule", "StartFrom" },
                values: new object[] { "Saturday 10:00", "Saturday 10:00" });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c2222222-2222-2222-2222-222222222222"),
                columns: new[] { "Schedule", "StartFrom" },
                values: new object[] { "Saturday 10:00", "Saturday 10:00" });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                columns: new[] { "Schedule", "StartFrom" },
                values: new object[] { "Saturday 10:00", "Saturday 10:00" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StartFrom",
                table: "Courses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "28a69e4b-bc4f-4321-a9b7-aecca4c26037", "AQAAAAIAAYagAAAAEN+zVriG1hw7qpjTAxkhmsMds68UEzAza6PR6QRRteeBxXzfwyHOUEz4sXl0KP9H+A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f47b797e-220a-4189-8d50-cd91d42197f1", "AQAAAAIAAYagAAAAEIYqbLc1m1BDk6yYNUxCSt9M3xNacL2EG+rLUoxsrmcwkU7ETFFD4v6VEDM86/2NIw==" });
        }
    }
}
