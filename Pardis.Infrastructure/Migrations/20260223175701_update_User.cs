using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "FatherName", "NationalCode", "PasswordHash" },
                values: new object[] { "0bcb724c-d56e-4768-9fd4-2993f2c5ca05", null, null, "AQAAAAIAAYagAAAAEAJbViGWH0U4i5PmS70B3RojQcWVlQUaqKmk0lla7glRErGiUeGU7FUh9U+0HnbK0Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "FatherName", "NationalCode", "PasswordHash" },
                values: new object[] { "9d1307f2-2f46-4db9-91c9-958dfed0924f", null, null, "AQAAAAIAAYagAAAAEA/20vz3/CDQ95e2uJQqGwjX9x7HlgIVcS8OxQaWdCMSco/Luxqu0f5C+1m3dEcr3A==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalCode",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cc87f543-658d-4ece-b6b7-a8f3c7ffed8b", "AQAAAAIAAYagAAAAENm+OCB0PzczXiHNgwF12n0w+ax5LGJ7EXcXlnJaKfZXMjtEvNiV2dgQnCO/m7PdqA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b54cc6e0-47b5-446f-a585-4d7dcaf56496", "AQAAAAIAAYagAAAAEJZlns52wy8rcOUQFffIVau1Ztx72uGHkNEyia949gbHABLTaFgx5hosyPWqUbCiyQ==" });
        }
    }
}
