using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManualPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManualPaymentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReceiptFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptUploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminReviewedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdminReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "PaymentSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSettings", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "35fc65bf-e9db-49bb-9dd1-dbb832c22900", "AQAAAAIAAYagAAAAENXic3J135fWo+e6SYYqXEmABrAX0iyyfQebrkXGT2vMlFc/2mJY5H4iA14HsYehbQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1f0c18e0-4817-4294-89d7-f29f6e13271c", "AQAAAAIAAYagAAAAEHtIJ+Vomz+GYCPNRFs7NDfaOcJJxPXshYKiNCDzNMHxQypMmcTvIvsiOeAOW0bB+A==" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManualPaymentRequests");

            migrationBuilder.DropTable(
                name: "PaymentSettings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fb993f1d-ff53-4022-9a41-be46c27e1550", "AQAAAAIAAYagAAAAELcOGknGXJhfSwvhTuAHkDQIsyryRKdqQEzPCVE71R3VHCJFgPthdgAi5jPmg92TxA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d1c3b573-378c-4a39-a93d-46da96d7c52c", "AQAAAAIAAYagAAAAEDF296Vk5YMhrmnIrjTNfDRHazm8KINA9ZqjUb8yqPAyg3xgY9wAqXLzsEGRk+VWOA==" });
        }
    }
}
