using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "BlogCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4cc3ea46-c871-4e7b-9d9f-d688f4404b0c", "AQAAAAIAAYagAAAAEE8oQmcRLG7MdkZT0q3p/eaX4vEhWPh2CY3JuUrUaiRsuahEvKnaituuaCIghCHU7A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3aea6584-9ed6-4ad2-b3a6-3f774f1f7e40", "AQAAAAIAAYagAAAAEOKMSsvTIDwXHuR6Yo6VfMtJjUgoWRRsDVqbEuloQKDqPIfxx4NpwH0TwkuM3brCPg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "BlogCategories");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "45c10d88-dffa-47f3-b155-c0f31768d894", "AQAAAAIAAYagAAAAEND7ESaocdvzJ2rGo7YsRAsv2HtmuXIu6DK7mDh2c0kTVKGQoWQUEoInjgjH/9I4gQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "900bdca3-1806-4699-bc69-60cd45e01f32", "AQAAAAIAAYagAAAAEGVFu3McpAx5UMSLBJxugUKNBTPkqZGi82773srQiboES/oc+2wyxt+9T1sQnZtxMw==" });
        }
    }
}
