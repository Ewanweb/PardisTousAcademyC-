using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateSlides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionLabel",
                table: "SuccessStories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionLink",
                table: "SuccessStories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Badge",
                table: "SuccessStories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "SuccessStories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatsJson",
                table: "SuccessStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "SuccessStories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SuccessStories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Badge",
                table: "HeroSlides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryActionLabel",
                table: "HeroSlides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryActionLink",
                table: "HeroSlides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryActionLabel",
                table: "HeroSlides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryActionLink",
                table: "HeroSlides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatsJson",
                table: "HeroSlides",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionLabel",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "ActionLink",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Badge",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "StatsJson",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Badge",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "PrimaryActionLabel",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "PrimaryActionLink",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "SecondaryActionLabel",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "SecondaryActionLink",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "StatsJson",
                table: "HeroSlides");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6ab978a4-37f9-444a-94c1-dc46e477db41", "AQAAAAIAAYagAAAAEKUqxVYkrg+DCPBRubJSRHSnWiWQn7MBo2VjN1eEXk25biGedYOH4SVJ9z+jNIUDtg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1e694f8d-a0e4-47c8-b206-8189e04cf2be", "AQAAAAIAAYagAAAAEHlwCR4Z2yMu2reI1zOpX0cpLHjasDh4KS7GW/gcmit9UyJpYHxxTrL4016vfe2ArQ==" });
        }
    }
}
