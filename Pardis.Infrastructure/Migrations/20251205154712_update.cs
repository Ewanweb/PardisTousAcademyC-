using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStarted",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3acf8e29-f5d4-44ef-a7b5-3a8712bbde45", "AQAAAAIAAYagAAAAEE5KmCUjG34phqlSCibb5Z8GVyRLmBK1F19t1Ig/0tZBnHbL5Ai1p2GM6zoItrz8UQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "51d448cf-05be-4f60-9a37-c9b2898f1a2d", "AQAAAAIAAYagAAAAEFeL4EVDNKRegk+2S2VYAAHSSQdVOHzdQfWcNm/+szIXVv1/OvYE5ud1vv+sapetzA==" });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c1111111-1111-1111-1111-111111111111"),
                columns: new[] { "IsCompleted", "IsStarted" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c2222222-2222-2222-2222-222222222222"),
                columns: new[] { "IsCompleted", "IsStarted" },
                values: new object[] { false, true });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                columns: new[] { "IsCompleted", "IsStarted" },
                values: new object[] { false, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsStarted",
                table: "Courses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bafa4ea9-1821-4271-8a66-52d26c3cbc3b", "AQAAAAIAAYagAAAAEP43mcM/oij0vTMTpwhhw/vKQf9PNJuu5M9QzW2JB7jgoFa5XYEsyciD8ikDMY9FwQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4dbd698f-214e-45bb-97a4-59396c8974d8", "AQAAAAIAAYagAAAAEPAwrf+rVrw4wxrXrj3OcQUuXFepq47wZuc/+usmky6lZ1q0gNjXPIXCNGNL2+qoUw==" });
        }
    }
}
