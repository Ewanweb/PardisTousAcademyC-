using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleIdToSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "CourseSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_ScheduleId",
                table: "CourseSessions",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_Schedules_ScheduleId",
                table: "CourseSessions",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_Schedules_ScheduleId",
                table: "CourseSessions");

            migrationBuilder.DropIndex(
                name: "IX_CourseSessions_ScheduleId",
                table: "CourseSessions");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "CourseSessions");
        }
    }
}