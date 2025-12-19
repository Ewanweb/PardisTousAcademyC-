using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdataDatab : Migration
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
                values: new object[] { "ce3d6478-0537-43cf-835b-0d07f4f2fde7", "AQAAAAIAAYagAAAAEO8NGBSojJ2a47ajs7K+40wP78R9MwPzjxQ+haz49Az2pMylTptxngVbBJJdj8s25Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3886164e-68fe-4eaf-83ae-e56ac9588a69", "AQAAAAIAAYagAAAAENE9TlGluQ/YE8Szfga9aRAESiXWOlmnTgpqqDEsUe/3n1UCx3fJxXfDt0Pggc1nzQ==" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_CourseSchedules_ScheduleId",
                table: "CourseSessions",
                column: "ScheduleId",
                principalTable: "CourseSchedules",
                principalColumn: "Id");
        }
    }
}
