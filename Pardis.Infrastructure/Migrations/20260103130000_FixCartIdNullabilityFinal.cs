using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCartIdNullabilityFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) First, safely drop the existing index if it exists
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

            // 2) Update NULL CartId values with unique GUIDs (NEVER use Guid.Empty)
            migrationBuilder.Sql(@"
                UPDATE o
                SET CartId = NEWID()
                FROM dbo.Orders o
                WHERE o.CartId IS NULL;
            ");

            // 3) Verify no NULL values remain
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM dbo.Orders WHERE CartId IS NULL)
                BEGIN
                    RAISERROR('Still have NULL CartId values after update', 16, 1);
                END
            ");

            // 4) Make CartId NOT NULL (no default value - must be explicitly set)
            migrationBuilder.AlterColumn<Guid>(
                name: "CartId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid?),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // 5) Recreate the unique index for active orders only
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX [IX_Orders_UserId_CartId_Active]
                ON [dbo].[Orders] ([UserId], [CartId])
                WHERE [Status] IN (0, 1);
            ");

            // 6) Add check constraint to prevent Guid.Empty (optional but recommended)
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[Orders]
                ADD CONSTRAINT [CK_Orders_CartId_NotEmpty]
                CHECK ([CartId] != '00000000-0000-0000-0000-000000000000');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1) Drop check constraint
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.check_constraints
                    WHERE name = N'CK_Orders_CartId_NotEmpty'
                      AND parent_object_id = OBJECT_ID(N'dbo.Orders')
                )
                BEGIN
                    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [CK_Orders_CartId_NotEmpty];
                END
            ");

            // 2) Drop the unique index
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

            // 3) Make CartId nullable again
            migrationBuilder.AlterColumn<Guid?>(
                name: "CartId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // 4) Recreate the old index with NULL filter
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX [IX_Orders_UserId_CartId_Active]
                ON [dbo].[Orders] ([UserId], [CartId])
                WHERE [Status] IN (0, 1) AND [CartId] IS NOT NULL;
            ");
        }
    }
}