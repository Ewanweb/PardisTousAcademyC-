using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class heroupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Badge",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "CourseName",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "IsPermanent",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "StatsJson",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "StudentName",
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
                name: "ButtonText",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "IsPermanent",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "PrimaryActionLabel",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "PrimaryActionLink",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "StatsJson",
                table: "HeroSlides");

            migrationBuilder.RenameColumn(
                name: "SecondaryActionLink",
                table: "HeroSlides",
                newName: "ActionLink");

            migrationBuilder.RenameColumn(
                name: "SecondaryActionLabel",
                table: "HeroSlides",
                newName: "ActionLabel");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SuccessStories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActionLink",
                table: "HeroSlides",
                newName: "SecondaryActionLink");

            migrationBuilder.RenameColumn(
                name: "ActionLabel",
                table: "HeroSlides",
                newName: "SecondaryActionLabel");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SuccessStories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Badge",
                table: "SuccessStories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "SuccessStories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseName",
                table: "SuccessStories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "SuccessStories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "SuccessStories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanent",
                table: "SuccessStories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "SuccessStories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatsJson",
                table: "SuccessStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "SuccessStories",
                type: "nvarchar(100)",
                maxLength: 100,
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
                name: "ButtonText",
                table: "HeroSlides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "HeroSlides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanent",
                table: "HeroSlides",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "HeroSlides",
                type: "nvarchar(500)",
                maxLength: 500,
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
                name: "StatsJson",
                table: "HeroSlides",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
