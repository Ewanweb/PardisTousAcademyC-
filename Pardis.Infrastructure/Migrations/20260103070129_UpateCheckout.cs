using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpateCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CartId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: ["ConcurrencyStamp", "PasswordHash"],
                values: new object[] { "4b484499-bf65-4cd5-8616-f043a335100b", "AQAAAAIAAYagAAAAEIA1QJU+oe5gtb3sGaeSZBbsdyJz4/RsUzJYIbgUDtL3YxakjPB2AHWIXCim8pTUpA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "03f094eb-b708-4324-a65b-90bd05ba6949", "AQAAAAIAAYagAAAAEBcei+bXhfNHbPMVnAaVBHfkicbLri9xkAypcwsjJ1ug5y9jE5uLtWz3OcWoZfyFUA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "03dd9f10-e386-4668-a22b-ceec30da886d", "AQAAAAIAAYagAAAAEL14yWgDGLs8ynK2EhN/V2W+8GZE3BDcN4oKctYruHk2LDGeGDiIC2ojb25mBl+iPw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "59635cdc-3547-4500-8134-0a1a7a1b27fe", "AQAAAAIAAYagAAAAECJWygS9UzjvcEhOLlR2HHLcXEjlHdZY1crMoc/inC2+ABjVcd4OxlvzuHnZ4VNNyA==" });
        }
    }
}
