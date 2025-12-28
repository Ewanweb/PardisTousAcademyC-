-- Rollback Script for Slider Simplification Migration
-- This script provides an alternative rollback method if the EF migration rollback fails
-- Run this script to restore the complex slider structure

BEGIN TRANSACTION;

PRINT 'Starting rollback of slider simplification...';

-- Step 1: Check if backup tables exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroSlides_PreSimplification_Backup')
BEGIN
    PRINT 'ERROR: HeroSlides backup table not found. Cannot perform rollback.';
    ROLLBACK TRANSACTION;
    RETURN;
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories_PreSimplification_Backup')
BEGIN
    PRINT 'ERROR: SuccessStories backup table not found. Cannot perform rollback.';
    ROLLBACK TRANSACTION;
    RETURN;
END

-- Step 2: Add back complex columns to HeroSlides if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'Badge')
    ALTER TABLE HeroSlides ADD Badge NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ButtonText')
    ALTER TABLE HeroSlides ADD ButtonText NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'LinkUrl')
    ALTER TABLE HeroSlides ADD LinkUrl NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'PrimaryActionLabel')
    ALTER TABLE HeroSlides ADD PrimaryActionLabel NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'PrimaryActionLink')
    ALTER TABLE HeroSlides ADD PrimaryActionLink NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'SecondaryActionLabel')
    ALTER TABLE HeroSlides ADD SecondaryActionLabel NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'SecondaryActionLink')
    ALTER TABLE HeroSlides ADD SecondaryActionLink NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'StatsJson')
    ALTER TABLE HeroSlides ADD StatsJson NVARCHAR(MAX) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'IsPermanent')
    ALTER TABLE HeroSlides ADD IsPermanent BIT NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ExpiresAt')
    ALTER TABLE HeroSlides ADD ExpiresAt DATETIME2 NULL;

-- Step 3: Add back complex columns to SuccessStories if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Badge')
    ALTER TABLE SuccessStories ADD Badge NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'LinkUrl')
    ALTER TABLE SuccessStories ADD LinkUrl NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'StudentName')
    ALTER TABLE SuccessStories ADD StudentName NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'CourseName')
    ALTER TABLE SuccessStories ADD CourseName NVARCHAR(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Subtitle')
    ALTER TABLE SuccessStories ADD Subtitle NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Type')
    ALTER TABLE SuccessStories ADD Type NVARCHAR(50) NOT NULL DEFAULT 'success';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'StatsJson')
    ALTER TABLE SuccessStories ADD StatsJson NVARCHAR(MAX) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Duration')
    ALTER TABLE SuccessStories ADD Duration INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'IsPermanent')
    ALTER TABLE SuccessStories ADD IsPermanent BIT NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'ExpiresAt')
    ALTER TABLE SuccessStories ADD ExpiresAt DATETIME2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'CourseId')
    ALTER TABLE SuccessStories ADD CourseId UNIQUEIDENTIFIER NULL;

-- Step 4: Restore Description column length for SuccessStories
ALTER TABLE SuccessStories ALTER COLUMN Description NVARCHAR(1000) NULL;

-- Step 5: Clear current data and restore from backup
DELETE FROM HeroSlides;
DELETE FROM SuccessStories;

-- Restore HeroSlides data
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

-- Restore SuccessStories data
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

-- Step 6: Remove simplified constraints if they exist
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_HeroSlides_Order_Simplified')
    ALTER TABLE HeroSlides DROP CONSTRAINT CK_HeroSlides_Order_Simplified;

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Order_Simplified')
    ALTER TABLE SuccessStories DROP CONSTRAINT CK_SuccessStories_Order_Simplified;

-- Step 7: Restore original constraints
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_HeroSlides_Order')
    ALTER TABLE HeroSlides ADD CONSTRAINT CK_HeroSlides_Order CHECK ([Order] >= 0);

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Order')
    ALTER TABLE SuccessStories ADD CONSTRAINT CK_SuccessStories_Order CHECK ([Order] >= 0);

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Duration')
    ALTER TABLE SuccessStories ADD CONSTRAINT CK_SuccessStories_Duration CHECK (Duration >= 1000 AND Duration <= 30000);

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Type')
    ALTER TABLE SuccessStories ADD CONSTRAINT CK_SuccessStories_Type CHECK (Type IN ('success', 'video'));

-- Step 8: Restore indexes for performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HeroSlides_IsActive_IsPermanent_Order')
    CREATE INDEX IX_HeroSlides_IsActive_IsPermanent_Order ON HeroSlides (IsActive, IsPermanent, [Order]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HeroSlides_ExpiresAt')
    CREATE INDEX IX_HeroSlides_ExpiresAt ON HeroSlides (ExpiresAt) WHERE ExpiresAt IS NOT NULL;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SuccessStories_IsActive_IsPermanent_Order')
    CREATE INDEX IX_SuccessStories_IsActive_IsPermanent_Order ON SuccessStories (IsActive, IsPermanent, [Order]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SuccessStories_ExpiresAt')
    CREATE INDEX IX_SuccessStories_ExpiresAt ON SuccessStories (ExpiresAt) WHERE ExpiresAt IS NOT NULL;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SuccessStories_Type')
    CREATE INDEX IX_SuccessStories_Type ON SuccessStories (Type);

-- Step 9: Clean up backup tables
DROP TABLE HeroSlides_PreSimplification_Backup;
DROP TABLE SuccessStories_PreSimplification_Backup;

-- Step 10: Remove ActionLabel and ActionLink columns from HeroSlides if they exist
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ActionLabel')
    ALTER TABLE HeroSlides DROP COLUMN ActionLabel;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ActionLink')
    ALTER TABLE HeroSlides DROP COLUMN ActionLink;

COMMIT TRANSACTION;

PRINT 'Rollback completed successfully. Complex slider structure has been restored.';
PRINT 'Backup tables have been cleaned up.';

-- Verification queries
PRINT 'Verification: Checking restored data counts...';
SELECT 'HeroSlides' as TableName, COUNT(*) as RecordCount FROM HeroSlides;
SELECT 'SuccessStories' as TableName, COUNT(*) as RecordCount FROM SuccessStories;

PRINT 'Verification: Checking column structure...';
SELECT 
    'HeroSlides' as TableName,
    COUNT(*) as ColumnCount
FROM sys.columns 
WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]');

SELECT 
    'SuccessStories' as TableName,
    COUNT(*) as ColumnCount
FROM sys.columns 
WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]');

PRINT 'Rollback verification completed.';