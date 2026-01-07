using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyPaymentSystemFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop ManualPaymentRequests table if it exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('ManualPaymentRequests', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE [ManualPaymentRequests];
                END
            ");

            // Drop index if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_UserId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    DROP INDEX [IX_Orders_UserId] ON [Orders];
                END
            ");

            // Drop columns if they exist
            migrationBuilder.Sql(@"
                IF COL_LENGTH('PaymentAttempts', 'ExpiresAt') IS NOT NULL
                BEGIN
                    ALTER TABLE [PaymentAttempts] DROP COLUMN [ExpiresAt];
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('PaymentAttempts', 'GatewayResponse') IS NOT NULL
                BEGIN
                    ALTER TABLE [PaymentAttempts] DROP COLUMN [GatewayResponse];
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('PaymentAttempts', 'ProviderReference') IS NOT NULL
                BEGIN
                    ALTER TABLE [PaymentAttempts] DROP COLUMN [ProviderReference];
                END
            ");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d8f57fc8-4a53-49e2-a87e-f055d934db38", "AQAAAAIAAYagAAAAEKGP63JWepNM4w32ShrwLwna4r4+uyGlEGgsO7qRvexxP561bHlOinysvz3QkDGDvg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ffcb7339-ff8b-4364-8c4b-f3d37de19ad2", "AQAAAAIAAYagAAAAEPIBjSls1Nr6/k9aANOmTcJFJloslJPdbuC2IL5JM3d8uF91BORVBvjokNi8KQw2Jg==" });

            // Create new index if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_UserId_CartId_Active' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Orders_UserId_CartId_Active] ON [Orders] ([UserId], [CartId]) WHERE [Status] IN (0, 1);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId_CartId_Active",
                table: "Orders");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "PaymentAttempts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayResponse",
                table: "PaymentAttempts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderReference",
                table: "PaymentAttempts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ManualPaymentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminReviewedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptUploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualPaymentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualPaymentRequests_AspNetUsers_AdminReviewedBy",
                        column: x => x.AdminReviewedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPaymentRequests_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualPaymentRequests_Courses_CourseId",
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
                values: new object[] { "1476d348-ccfa-44c3-a1bb-ebbb4f44fa80", "AQAAAAIAAYagAAAAECc9H1Jkf+Alu/ZcsEZwYnPTAhdX2qu5T/yixbNgppXJGAlf8ptHs2ghtIptFo6Nsg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0992eaac-bd94-4a86-8cc0-fd712689cdc5", "AQAAAAIAAYagAAAAEKvqmNpFVPuUa8KAkDiUYOVIoBbYXaxx8VaQ/Mg4ozcHUYLmQAE72X6wfkAzcdz8Bg==" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentRequests_AdminReviewedBy",
                table: "ManualPaymentRequests",
                column: "AdminReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentRequests_CourseId",
                table: "ManualPaymentRequests",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPaymentRequests_StudentId",
                table: "ManualPaymentRequests",
                column: "StudentId");
        }
    }
}
