using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpateCheckout2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");

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

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId_CartId_Active",
                table: "Orders",
                columns: new[] { "UserId", "CartId" },
                unique: true,
                filter: "[Status] IN (0, 1) AND [CartId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId_CartId_Active",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4b484499-bf65-4cd5-8616-f043a335100b", "AQAAAAIAAYagAAAAEIA1QJU+oe5gtb3sGaeSZBbsdyJz4/RsUzJYIbgUDtL3YxakjPB2AHWIXCim8pTUpA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "03f094eb-b708-4324-a65b-90bd05ba6949", "AQAAAAIAAYagAAAAEBcei+bXhfNHbPMVnAaVBHfkicbLri9xkAypcwsjJ1ug5y9jE5uLtWz3OcWoZfyFUA==" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");
        }
    }
}
