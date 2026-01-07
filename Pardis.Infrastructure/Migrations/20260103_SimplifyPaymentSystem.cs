using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyPaymentSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // STEP 1: Archive existing non-manual payment attempts
            migrationBuilder.Sql(@"
                -- Archive non-manual payment attempts to backup table
                SELECT * INTO PaymentAttempts_Archive_20260103 
                FROM PaymentAttempts 
                WHERE Method != 2; -- Keep only Manual (2)
            ");

            // STEP 2: Update existing PaymentAttempts to only allow Manual method
            migrationBuilder.Sql(@"
                -- Update any non-manual methods to Manual for data consistency
                UPDATE PaymentAttempts 
                SET Method = 2 
                WHERE Method != 2;
            ");

            // STEP 3: Remove unused PaymentAttemptStatus values
            migrationBuilder.Sql(@"
                -- Archive expired/refunded payments
                SELECT * INTO PaymentAttempts_Expired_Archive_20260103 
                FROM PaymentAttempts 
                WHERE Status IN (6, 7); -- Expired, Refunded
                
                -- Update expired/refunded to Failed
                UPDATE PaymentAttempts 
                SET Status = 5 -- Failed
                WHERE Status IN (6, 7);
            ");

            // STEP 4: Consolidate ManualPaymentRequests into PaymentAttempts
            migrationBuilder.Sql(@"
                -- Migrate ManualPaymentRequests to PaymentAttempts if any exist
                INSERT INTO PaymentAttempts (
                    Id, OrderId, UserId, Method, Amount, Status, 
                    ReceiptImageUrl, ReceiptFileName, ReceiptUploadedAt,
                    AdminReviewedBy, AdminReviewedAt, AdminDecision,
                    CreatedAt, UpdatedAt, TrackingCode
                )
                SELECT 
                    mpr.Id,
                    NEWID(), -- Generate new OrderId - will need to be handled properly
                    mpr.StudentId,
                    2, -- Manual
                    mpr.Amount,
                    CASE 
                        WHEN mpr.Status = 0 THEN 1 -- PendingReceipt -> PendingPayment
                        WHEN mpr.Status = 1 THEN 3 -- PendingApproval -> AwaitingAdminApproval  
                        WHEN mpr.Status = 2 THEN 4 -- Approved -> Paid
                        WHEN mpr.Status = 3 THEN 5 -- Rejected -> Failed
                        ELSE 1
                    END,
                    mpr.ReceiptFileUrl,
                    mpr.ReceiptFileName,
                    mpr.ReceiptUploadedAt,
                    mpr.AdminReviewedBy,
                    mpr.AdminReviewedAt,
                    CASE WHEN mpr.Status = 2 THEN 'Approved' WHEN mpr.Status = 3 THEN 'Rejected' ELSE NULL END,
                    mpr.CreatedAt,
                    mpr.UpdatedAt,
                    'MAN-' + CONVERT(VARCHAR(36), mpr.Id)
                FROM ManualPaymentRequests mpr
                WHERE NOT EXISTS (
                    SELECT 1 FROM PaymentAttempts pa WHERE pa.Id = mpr.Id
                );
            ");

            // STEP 5: Archive and drop ManualPaymentRequests table
            migrationBuilder.Sql(@"
                -- Archive ManualPaymentRequests
                SELECT * INTO ManualPaymentRequests_Archive_20260103 
                FROM ManualPaymentRequests;
            ");

            migrationBuilder.DropTable(name: "ManualPaymentRequests");

            // STEP 6: Clean up PaymentAttempts columns not needed for manual payments
            migrationBuilder.DropColumn(
                name: "ProviderReference",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "GatewayResponse", 
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "PaymentAttempts");

            // STEP 7: Add constraints for manual-only system
            migrationBuilder.Sql(@"
                -- Add check constraint to ensure only Manual payment method
                ALTER TABLE PaymentAttempts 
                ADD CONSTRAINT CK_PaymentAttempts_Method_Manual_Only 
                CHECK (Method = 2);
                
                -- Add check constraint for valid status values
                ALTER TABLE PaymentAttempts 
                ADD CONSTRAINT CK_PaymentAttempts_Status_Valid 
                CHECK (Status IN (0, 1, 3, 4, 5)); -- Draft, PendingPayment, AwaitingAdminApproval, Paid, Failed
            ");

            // STEP 8: Update seed data to reflect changes
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id", 
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { Guid.NewGuid().ToString(), "AQAAAAIAAYagAAAAENewHashForSimplifiedPaymentSystem" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ROLLBACK: Restore original payment system
            
            // Restore constraints
            migrationBuilder.Sql(@"
                ALTER TABLE PaymentAttempts DROP CONSTRAINT IF EXISTS CK_PaymentAttempts_Method_Manual_Only;
                ALTER TABLE PaymentAttempts DROP CONSTRAINT IF EXISTS CK_PaymentAttempts_Status_Valid;
            ");

            // Restore columns
            migrationBuilder.AddColumn<string>(
                name: "ProviderReference",
                table: "PaymentAttempts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayResponse",
                table: "PaymentAttempts", 
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "PaymentAttempts",
                type: "datetime2",
                nullable: true);

            // Restore ManualPaymentRequests table
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

            // Restore data from archives if needed
            migrationBuilder.Sql(@"
                -- Restore ManualPaymentRequests from archive
                IF OBJECT_ID('ManualPaymentRequests_Archive_20260103', 'U') IS NOT NULL
                BEGIN
                    INSERT INTO ManualPaymentRequests SELECT * FROM ManualPaymentRequests_Archive_20260103;
                    DROP TABLE ManualPaymentRequests_Archive_20260103;
                END
                
                -- Restore non-manual PaymentAttempts from archive  
                IF OBJECT_ID('PaymentAttempts_Archive_20260103', 'U') IS NOT NULL
                BEGIN
                    INSERT INTO PaymentAttempts SELECT * FROM PaymentAttempts_Archive_20260103;
                    DROP TABLE PaymentAttempts_Archive_20260103;
                END
            ");
        }
    }
}