using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCartIdNullability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) داده‌ها: CartId های NULL را یکتا کن تا Unique Index خراب نشود
            migrationBuilder.Sql(@"
                UPDATE o
                SET CartId = NEWID()
                FROM dbo.Orders o
                WHERE o.CartId IS NULL;
            ");

            // 2) اگر ایندکس وجود دارد، Drop کن (ایمن)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Orders_UserId_CartId_Active'
                      AND object_id = OBJECT_ID(N'dbo.Orders')
                )
                BEGIN
                    DROP INDEX [IX_Orders_UserId_CartId_Active] ON [dbo].[Orders];
                END
            ");

            // 3) ستون را NOT NULL کن (بدون default Guid.Empty)
            migrationBuilder.AlterColumn<Guid>(
                name: "CartId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // 4) ایندکس یکتا را دوباره بساز (اگر وجود ندارد) - روی سفارش‌های فعال
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Orders_UserId_CartId_Active'
                      AND object_id = OBJECT_ID(N'dbo.Orders')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Orders_UserId_CartId_Active]
                    ON [dbo].[Orders] ([UserId], [CartId])
                    WHERE [Status] IN (0, 1);
                END
            ");

            // 5) UpdateData ها (همان الگوی EF)
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[]
                {
                    "fix-cartid-stamp-1",
                    "AQAAAAIAAYagAAAAEJxn4qSdV2W2h+KBuC+q/1SQkRWYfnXjBRcQu84WdD4MJ2GCoUKQ+ricUv7Bjq5lEw=="
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[]
                {
                    "fix-cartid-stamp-2",
                    "AQAAAAIAAYagAAAAEMRxaxg+XvJLSHWcJ+i/oZx4ReeMVJ2sXDdO+w2zFxTOb4ZK9WZueaV0N35lOBk63w=="
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1) Drop index اگر وجود دارد (ایمن)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Orders_UserId_CartId_Active'
                      AND object_id = OBJECT_ID(N'dbo.Orders')
                )
                BEGIN
                    DROP INDEX [IX_Orders_UserId_CartId_Active] ON [dbo].[Orders];
                END
            ");

            // 2) CartId را دوباره Nullable کن
            migrationBuilder.AlterColumn<Guid?>(
                name: "CartId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // 3) ایندکس قبلی را دوباره بساز (فقط وقتی CartId null نیست)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Orders_UserId_CartId_Active'
                      AND object_id = OBJECT_ID(N'dbo.Orders')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Orders_UserId_CartId_Active]
                    ON [dbo].[Orders] ([UserId], [CartId])
                    WHERE [Status] IN (0, 1) AND [CartId] IS NOT NULL;
                END
            ");

            // 4) Revert UpdateData ها
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[]
                {
                    "eaa52ee6-37e9-472b-927d-3c81e609c6f4",
                    "AQAAAAIAAYagAAAAEJxn4qSdV2W2h+KBuC+q/1SQkRWYfnXjBRcQu84WdD4MJ2GCoUKQ+ricUv7Bjq5lEw=="
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[]
                {
                    "1ea8a05e-66c9-40c1-afa9-c29f71d3efc8",
                    "AQAAAAIAAYagAAAAEMRxaxg+XvJLSHWcJ+i/oZx4ReeMVJ2sXDdO+w2zFxTOb4ZK9WZueaV0N35lOBk63w=="
                });
        }
    }
}