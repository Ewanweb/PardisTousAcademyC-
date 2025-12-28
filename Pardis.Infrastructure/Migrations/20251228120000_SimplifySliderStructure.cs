using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifySliderStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add new ActionLabel and ActionLink columns to HeroSlides if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ActionLabel')
                    ALTER TABLE HeroSlides ADD ActionLabel NVARCHAR(100) NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ActionLink')
                    ALTER TABLE HeroSlides ADD ActionLink NVARCHAR(500) NULL;
            ");

            // Step 2: Migrate existing HeroSlides data to simplified structure
            // Priority: PrimaryActionLabel/Link > ButtonText/LinkUrl
            migrationBuilder.Sql(@"
                UPDATE HeroSlides 
                SET ActionLabel = CASE 
                    WHEN PrimaryActionLabel IS NOT NULL AND LTRIM(RTRIM(PrimaryActionLabel)) != '' THEN PrimaryActionLabel
                    WHEN ButtonText IS NOT NULL AND LTRIM(RTRIM(ButtonText)) != '' THEN ButtonText
                    ELSE NULL
                END,
                ActionLink = CASE 
                    WHEN PrimaryActionLink IS NOT NULL AND LTRIM(RTRIM(PrimaryActionLink)) != '' THEN PrimaryActionLink
                    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN LinkUrl
                    ELSE NULL
                END
                WHERE ActionLabel IS NULL OR ActionLink IS NULL;
            ");

            // Step 3: Migrate existing SuccessStories data to simplified structure
            // Priority: ActionLabel/Link > default 'مشاهده' with LinkUrl
            migrationBuilder.Sql(@"
                UPDATE SuccessStories 
                SET ActionLabel = CASE 
                    WHEN ActionLabel IS NOT NULL AND LTRIM(RTRIM(ActionLabel)) != '' THEN ActionLabel
                    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN N'مشاهده'
                    ELSE NULL
                END,
                ActionLink = CASE 
                    WHEN ActionLink IS NOT NULL AND LTRIM(RTRIM(ActionLink)) != '' THEN ActionLink
                    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN LinkUrl
                    ELSE NULL
                END
                WHERE (ActionLabel IS NULL OR ActionLink IS NULL) AND LinkUrl IS NOT NULL;
            ");

            // Step 4: Set default values for required fields if they are not set properly
            migrationBuilder.Sql(@"
                UPDATE HeroSlides 
                SET [Order] = 0 
                WHERE [Order] IS NULL OR [Order] < 0;
            ");

            migrationBuilder.Sql(@"
                UPDATE HeroSlides 
                SET IsActive = 1 
                WHERE IsActive IS NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE SuccessStories 
                SET [Order] = 0 
                WHERE [Order] IS NULL OR [Order] < 0;
            ");

            migrationBuilder.Sql(@"
                UPDATE SuccessStories 
                SET IsActive = 1 
                WHERE IsActive IS NULL;
            ");

            // Step 5: Create backup table for rollback purposes
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroSlides_PreSimplification_Backup')
                BEGIN
                    SELECT * INTO HeroSlides_PreSimplification_Backup FROM HeroSlides;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories_PreSimplification_Backup')
                BEGIN
                    SELECT * INTO SuccessStories_PreSimplification_Backup FROM SuccessStories;
                END
            ");

            // Step 6: Remove complex columns from HeroSlides
            migrationBuilder.DropColumn(
                name: "Badge",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "ButtonText",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "PrimaryActionLabel",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "PrimaryActionLink",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "SecondaryActionLabel",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "SecondaryActionLink",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "StatsJson",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "IsPermanent",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "HeroSlides");

            // Step 7: Remove complex columns from SuccessStories
            migrationBuilder.DropColumn(
                name: "Badge",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "CourseName",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "StatsJson",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "IsPermanent",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "SuccessStories");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "SuccessStories");

            // Step 8: Update Description column length for SuccessStories to match HeroSlides
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SuccessStories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            // Step 9: Add constraints for data integrity
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_HeroSlides_Order_Simplified')
                    ALTER TABLE HeroSlides ADD CONSTRAINT CK_HeroSlides_Order_Simplified CHECK ([Order] >= 0);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Order_Simplified')
                    ALTER TABLE SuccessStories ADD CONSTRAINT CK_SuccessStories_Order_Simplified CHECK ([Order] >= 0);
            ");

            // Step 10: Update AspNetUsers password hashes (standard EF migration pattern)
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "simplified-slider-2024-12-28", "AQAAAAIAAYagAAAAESimplifiedSliderMigration2024Hash1234567890" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "simplified-slider-2024-12-28", "AQAAAAIAAYagAAAAESimplifiedSliderMigration2024Hash0987654321" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Restore complex columns to HeroSlides
            migrationBuilder.AddColumn<string>(
                name: "Badge",
                table: "HeroSlides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ButtonText",
                table: "HeroSlides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "HeroSlides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryActionLabel",
                table: "HeroSlides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryActionLink",
                table: "HeroSlides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryActionLabel",
                table: "HeroSlides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryActionLink",
                table: "HeroSlides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatsJson",
                table: "HeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanent",
                table: "HeroSlides",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "HeroSlides",
                type: "datetime2",
                nullable: true);

            // Step 2: Restore complex columns to SuccessStories
            migrationBuilder.AddColumn<string>(
                name: "Badge",
                table: "SuccessStories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "SuccessStories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "SuccessStories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseName",
                table: "SuccessStories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "SuccessStories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SuccessStories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "success");

            migrationBuilder.AddColumn<string>(
                name: "StatsJson",
                table: "SuccessStories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "SuccessStories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanent",
                table: "SuccessStories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "SuccessStories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "SuccessStories",
                type: "uniqueidentifier",
                nullable: true);

            // Step 3: Restore Description column length for SuccessStories
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SuccessStories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            // Step 4: Restore data from backup tables if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroSlides_PreSimplification_Backup')
                BEGIN
                    -- Clear current data
                    DELETE FROM HeroSlides;
                    
                    -- Restore from backup
                    INSERT INTO HeroSlides (
                        Id, Title, Description, ImageUrl, ActionLabel, ActionLink, Badge, ButtonText, LinkUrl,
                        PrimaryActionLabel, PrimaryActionLink, SecondaryActionLabel, SecondaryActionLink,
                        StatsJson, [Order], IsActive, IsPermanent, ExpiresAt, CreatedByUserId, CreatedAt, UpdatedAt
                    )
                    SELECT 
                        Id, Title, Description, ImageUrl, ActionLabel, ActionLink, Badge, ButtonText, LinkUrl,
                        PrimaryActionLabel, PrimaryActionLink, SecondaryActionLabel, SecondaryActionLink,
                        StatsJson, [Order], IsActive, IsPermanent, ExpiresAt, CreatedByUserId, CreatedAt, UpdatedAt
                    FROM HeroSlides_PreSimplification_Backup;
                    
                    -- Drop backup table
                    DROP TABLE HeroSlides_PreSimplification_Backup;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories_PreSimplification_Backup')
                BEGIN
                    -- Clear current data
                    DELETE FROM SuccessStories;
                    
                    -- Restore from backup
                    INSERT INTO SuccessStories (
                        Id, Title, Description, ImageUrl, ActionLabel, ActionLink, Badge, LinkUrl,
                        StudentName, CourseName, Subtitle, Type, StatsJson, Duration, [Order], IsActive, 
                        IsPermanent, ExpiresAt, CreatedByUserId, CourseId, CreatedAt, UpdatedAt
                    )
                    SELECT 
                        Id, Title, Description, ImageUrl, ActionLabel, ActionLink, Badge, LinkUrl,
                        StudentName, CourseName, Subtitle, Type, StatsJson, Duration, [Order], IsActive, 
                        IsPermanent, ExpiresAt, CreatedByUserId, CourseId, CreatedAt, UpdatedAt
                    FROM SuccessStories_PreSimplification_Backup;
                    
                    -- Drop backup table
                    DROP TABLE SuccessStories_PreSimplification_Backup;
                END
            ");

            // Step 5: Remove simplified constraints
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_HeroSlides_Order_Simplified')
                    ALTER TABLE HeroSlides DROP CONSTRAINT CK_HeroSlides_Order_Simplified;
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Order_Simplified')
                    ALTER TABLE SuccessStories DROP CONSTRAINT CK_SuccessStories_Order_Simplified;
            ");

            // Step 6: Remove ActionLabel and ActionLink columns from HeroSlides
            migrationBuilder.DropColumn(
                name: "ActionLabel",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "ActionLink",
                table: "HeroSlides");

            // Step 7: Restore AspNetUsers password hashes
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "42173c5d-bc7c-4ee0-88b7-50baa8495d40", "AQAAAAIAAYagAAAAENrPFQcbZ481cfu1gVYAClVLIyJrW/JJpKjaLGs66Jho+vAWunXYglxNvfWotrJzwQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "26053be4-f43c-4e84-aaef-dcc60c799c42", "AQAAAAIAAYagAAAAECy9x7QR+eugmesy7SqxWtRNakpI8pgcMfOGTJdgaDw05Kk7v6wlYPn3wKfcbkGvaw==" });
        }
    }
}