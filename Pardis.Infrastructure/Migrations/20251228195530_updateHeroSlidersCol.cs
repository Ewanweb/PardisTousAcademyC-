using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateHeroSlidersCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fb993f1d-ff53-4022-9a41-be46c27e1550", "AQAAAAIAAYagAAAAELcOGknGXJhfSwvhTuAHkDQIsyryRKdqQEzPCVE71R3VHCJFgPthdgAi5jPmg92TxA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d1c3b573-378c-4a39-a93d-46da96d7c52c", "AQAAAAIAAYagAAAAEDF296Vk5YMhrmnIrjTNfDRHazm8KINA9ZqjUb8yqPAyg3xgY9wAqXLzsEGRk+VWOA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "9e10771b-a80a-42fd-ae12-3dbd7b9dec22", "AQAAAAIAAYagAAAAEHQFohabYgyFVQglffagYZr/V+A/+yN5jxujQEYz5d7ri6C+u3G+n6VUOFxfr+pQoQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b0d39832-46ea-4063-9317-930fd392801c", "AQAAAAIAAYagAAAAEFmrvsFgY9gAkvYyxik7vMqz2SbnML5bsI3MSYYjme6z0kE8hWvzsapzoDWYK1efzw==" });
        }
    }
}
