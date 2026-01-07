using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpateCheckout3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "33d7cc67-e45a-4227-8aa6-384b122a6da9", "AQAAAAIAAYagAAAAECMY3QWtftDOPPDSwwjfJTzgJUn0a0F1Hk9x+9k3zUj6PXOffyoliU6UTAZoCXVmdQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "79dcec73-d4a3-499d-9ac0-d18e0146aeaf", "AQAAAAIAAYagAAAAEBduF3SobmQHoNmbyNhoVdLjZIixy2kR1S873+ZixiCs2bzQADWbqAQ2j/sqse4ABQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "eaa52ee6-37e9-472b-927d-3c81e609c6f4", "AQAAAAIAAYagAAAAEJxn4qSdV2W2h+KBuC+q/1SQkRWYfnXjBRcQu84WdD4MJ2GCoUKQ+ricUv7Bjq5lEw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1ea8a05e-66c9-40c1-afa9-c29f71d3efc8", "AQAAAAIAAYagAAAAEMRxaxg+XvJLSHWcJ+i/oZx4ReeMVJ2sXDdO+w2zFxTOb4ZK9WZueaV0N35lOBk63w==" });
        }
    }
}
