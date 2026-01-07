using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bde0efa2-e452-4f23-8f1a-1dddf918867e", "AQAAAAIAAYagAAAAEPI2PIjuesaYzE5D30pbOR+ezSMurjV9nNM5tYXprR0enkcAV2KUohS3vEZgEKJKiQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "47d0f548-3c24-467f-81bf-63ac1db07c4e", "AQAAAAIAAYagAAAAECwK4tidDdBLsHAOj2kAqXeipRHTgYhm/hXoUuAmotrisfmaIsy++sKLPUs9hZlzVw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d8f57fc8-4a53-49e2-a87e-f055d934db38", "AQAAAAIAAYagAAAAEKGP63JWepNM4w32ShrwLwna4r4+uyGlEGgsO7qRvexxP561bHlOinysvz3QkDGDvg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ffcb7339-ff8b-4364-8c4b-f3d37de19ad2", "AQAAAAIAAYagAAAAEPIBjSls1Nr6/k9aANOmTcJFJloslJPdbuC2IL5JM3d8uF91BORVBvjokNi8KQw2Jg==" });
        }
    }
}
