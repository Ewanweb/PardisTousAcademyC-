using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_CourseSchedules_ScheduleId",
                table: "CourseSessions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "aba9a4a2-1c34-40fb-bf39-5984d9eb3615", "AQAAAAIAAYagAAAAEMnw14ewSykqtvQylpKqkQRznW9r7fhEy+9Lnv3+ChtSJhaLiByXTsEHHxoG2S+65g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d284be3e-664b-45cc-9f05-cbc451257794", "AQAAAAIAAYagAAAAEJzwIesc+7j+ul7+NHF/Rretatw4Ap4nhHpfj6uxosSdo3nhjtq5fv5yOHUxtqTrbg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_CourseSchedules_ScheduleId",
                table: "CourseSessions",
                column: "ScheduleId",
                principalTable: "CourseSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_CourseSchedules_ScheduleId",
                table: "CourseSessions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "54f5dd4e-fd0e-4339-87f9-a53cf4daaba9", "AQAAAAIAAYagAAAAEC6I1xNEydb59SsDF2bkQnPbMKdg4O+VdkfE2REsXGo3BMGLmYZUN7I2xDDimz4L4Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d9b13004-ed7e-4b2c-84f0-0dc2f0898277", "AQAAAAIAAYagAAAAEMVL+jJ7iIfA5Ol4FYmJGOUnwsyE0fU84IR1cUzFLAc4k1W+YSJv/4K5ULh0eqWM4w==" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_CourseSchedules_ScheduleId",
                table: "CourseSessions",
                column: "ScheduleId",
                principalTable: "CourseSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
