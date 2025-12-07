using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StartFrom",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StartFrom",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
        }
    }
}
