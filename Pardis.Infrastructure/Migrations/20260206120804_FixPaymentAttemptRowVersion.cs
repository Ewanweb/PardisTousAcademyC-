using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPaymentAttemptRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing RowVersion column
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PaymentAttempts");

            // Add it back as rowversion
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PaymentAttempts",
                type: "rowversion",
                rowVersion: true,
                nullable: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0f1cfdb5-9e3a-44b9-a30b-d02ccfed4391", "AQAAAAIAAYagAAAAEGkJ2KkzOFeUbFQSVoqYXYW7FD2/Y8zOSqBMorIKCJ9/A7X2TDhKs+IjvlpUn1BO7g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e4679165-b6a7-4e61-b77e-a7795f783f18", "AQAAAAIAAYagAAAAEHrKGbI4HTxAd2uPGdWrLfF/Zl6TuPEuN+CYXRaFep361qMgF+NHUw85eSSv+mc8yQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the rowversion column
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PaymentAttempts");

            // Add it back as varbinary(max)
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PaymentAttempts",
                type: "varbinary(max)",
                nullable: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "68ccf66c-e405-4a30-91a7-5ccf6ccef32d", "AQAAAAIAAYagAAAAED3vvhG0sQHC02y+/h4FdeWgvnj0Or+Ko9IpY1gCQiW5QRwdkzRrYyAWjD0+KSjjeQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1d5a6ecc-e0e8-4a24-98cc-68e70934c65c", "AQAAAAIAAYagAAAAEOG+wWIDccoZOTD/KosOPX8cH9lxO1mlLLOCdgx2nKHX0rNXJW0ggu0M38ss6VIxlA==" });
        }
    }
}
