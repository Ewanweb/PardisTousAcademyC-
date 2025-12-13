using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCourseFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCourses",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasePrice = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttendedSessionsCount = table.Column<int>(type: "int", nullable: false),
                    AbsentSessionsCount = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExclusiveLiveLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAccessBlocked = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalGrade = table.Column<int>(type: "int", nullable: true),
                    CertificateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstructorNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourses", x => new { x.UserId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_UserCourses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4bb940ac-c60d-4772-9acc-b0ee8c3e7bcf", "AQAAAAIAAYagAAAAEPws3GMAamoUEppk4pV5e0QiVJNDo7qtNMyCV7yeJ17xznhNy0OjR28sdWZCC5Z84A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ccc2ce25-e130-4c66-8247-80b7da6c0c6a", "AQAAAAIAAYagAAAAEBIb5SF0spI316EgOHzFLyRbhActWD+qN6MKNdvj6f4+hMwGbJPNNqJUctbp8od7NQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_UserCourses_CourseId",
                table: "UserCourses",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCourses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6851a99b-7c0e-40eb-8e61-2a5a352920a0", "AQAAAAIAAYagAAAAEGOZWEx+rcyW/hpkJWCYVhR0CT++SaZVaeaqLZ85+kOvYqDL1YG3WH91lcN1Pjp9qw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c8300fb1-3e59-4a15-8c4f-caeef067c324", "AQAAAAIAAYagAAAAED9pVzZGDhx+0AG2JjJ7+8VomfzOqw7QmrRC6ll3cqV2S7fC65SIL98SMBGkZPQt0A==" });
        }
    }
}
