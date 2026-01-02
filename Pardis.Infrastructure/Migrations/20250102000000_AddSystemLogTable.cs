using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EventId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_Level_Time",
                table: "SystemLogs",
                columns: new[] { "Level", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_Source_Time",
                table: "SystemLogs",
                columns: new[] { "Source", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_Time",
                table: "SystemLogs",
                column: "Time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemLogs");
        }
    }
}