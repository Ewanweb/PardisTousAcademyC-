using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdataData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "CourseSessions",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_ScheduleId",
                table: "CourseSessions",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_CourseSchedules_ScheduleId",
                table: "CourseSessions",
                column: "ScheduleId",
                principalTable: "CourseSchedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_CourseSchedules_ScheduleId",
                table: "CourseSessions");

            migrationBuilder.DropIndex(
                name: "IX_CourseSessions_ScheduleId",
                table: "CourseSessions");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "CourseSessions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "61f1da43-e595-48f9-ae49-a7137f1d00f3", "AQAAAAIAAYagAAAAEMpEz0lOT2g25896dQbZhDfH+qBRkFg3RDDHZGjTZYZXbV3+wyTVS/XhPPAKBUq9SQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "53143eee-c115-48d2-af13-c45954e5d6cb", "AQAAAAIAAYagAAAAEPVeM5RpOwTnwvtmB74JJqwhSpeg66qHOQppjdE+ZE0fbzWTCCOmxPZ/HYhrVuSLYA==" });
        }
    }
}
