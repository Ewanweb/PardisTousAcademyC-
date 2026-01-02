using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCartAndLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalAmount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalAmount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CartSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitPrice = table.Column<long>(type: "bigint", nullable: false),
                    TitleSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThumbnailSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstructorSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProviderReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackingCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiptImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptUploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "03dd9f10-e386-4668-a22b-ceec30da886d", "AQAAAAIAAYagAAAAEL14yWgDGLs8ynK2EhN/V2W+8GZE3BDcN4oKctYruHk2LDGeGDiIC2ojb25mBl+iPw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "59635cdc-3547-4500-8134-0a1a7a1b27fe", "AQAAAAIAAYagAAAAECJWygS9UzjvcEhOLlR2HHLcXEjlHdZY1crMoc/inC2+ABjVcd4OxlvzuHnZ4VNNyA==" });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "CreatedAt", "Description", "IsPublic", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "شماره کارت مقصد برای پرداخت دستی", true, "ManualPayment.CardNumber", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "6037-9977-****-****" },
                    { new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "نام صاحب کارت", true, "ManualPayment.CardHolder", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "آکادمی پردیس توس" },
                    { new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "نام بانک", true, "ManualPayment.BankName", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "بانک پاسارگاد" },
                    { new Guid("d4444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "توضیحات پرداخت دستی", true, "ManualPayment.Description", new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "لطفاً پس از واریز، رسید پرداخت را آپلود کنید" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_CourseId",
                table: "CartItems",
                columns: new[] { "CartId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CourseId",
                table: "CartItems",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_OrderId",
                table: "PaymentAttempts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_TrackingCode",
                table: "PaymentAttempts",
                column: "TrackingCode",
                unique: true,
                filter: "[TrackingCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_UserId",
                table: "PaymentAttempts",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

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
        }
    }
}
