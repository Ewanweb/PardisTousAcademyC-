using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManualPaymentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create PaymentSettings table
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

            // Create ManualPaymentRequests table
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

            // Create indexes
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

            // Insert default payment settings
            migrationBuilder.InsertData(
                table: "PaymentSettings",
                columns: new[] { "Id", "CardNumber", "CardHolderName", "BankName", "Description", "IsActive", "CreatedAt", "UpdatedAt" },
                values: new object[] { 
                    Guid.NewGuid(), 
                    "6037-9977-****-****", 
                    "آکادمی پردیس توس", 
                    "بانک کشاورزی", 
                    "لطفاً پس از واریز، رسید را در همین صفحه آپلود کنید", 
                    true, 
                    DateTime.UtcNow, 
                    DateTime.UtcNow 
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManualPaymentRequests");

            migrationBuilder.DropTable(
                name: "PaymentSettings");
        }
    }
}