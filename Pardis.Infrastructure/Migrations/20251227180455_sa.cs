using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "8c39ac75-b27e-428f-b13c-43a288ddc350", "AQAAAAIAAYagAAAAELl0+Ck0C2u19cFcUeLmgTCLaz89wA8jYDeqT17OHFqN/+6FooSRdt3zn4YHNm1OSQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "25780237-652c-4296-a1a9-197ca6b396e7", "AQAAAAIAAYagAAAAEPt6WaYMolhwykxuzFBzpHeLxZyckesmR812SZnii6QSx14f34EEBa7GyRblFz1TbA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "26053be4-f43c-4e84-aaef-dcc60c799c42", "AQAAAAIAAYagAAAAECy9x7QR+eugmesy7SqxWtRNakpI8pgcMfOGTJdgaDw05Kk7v6wlYPn3wKfcbkGvaw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "42173c5d-bc7c-4ee0-88b7-50baa8495d40", "AQAAAAIAAYagAAAAENrPFQcbZ481cfu1gVYAClVLIyJrW/JJpKjaLGs66Jho+vAWunXYglxNvfWotrJzwQ==" });
        }
    }
}
