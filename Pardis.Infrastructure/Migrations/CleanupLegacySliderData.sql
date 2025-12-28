-- =========================================================
-- Cleanup Legacy Slider Data and Backup Tables
-- This script removes backup tables and legacy migration artifacts
-- after successful slider system simplification
-- =========================================================

BEGIN TRANSACTION;

PRINT 'Starting cleanup of legacy slider data and backup tables...';
PRINT '';

-- Step 1: Remove backup tables if they exist
PRINT '=== REMOVING BACKUP TABLES ===';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroSlides_PreSimplification_Backup')
BEGIN
    DROP TABLE HeroSlides_PreSimplification_Backup;
    PRINT '✓ Removed HeroSlides_PreSimplification_Backup table';
END
ELSE
    PRINT '• HeroSlides backup table already removed';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories_PreSimplification_Backup')
BEGIN
    DROP TABLE SuccessStories_PreSimplification_Backup;
    PRINT '✓ Removed SuccessStories_PreSimplification_Backup table';
END
ELSE
    PRINT '• SuccessStories backup table already removed';

PRINT '';

-- Step 2: Verify current simplified structure
PRINT '=== VERIFYING SIMPLIFIED STRUCTURE ===';

-- Check HeroSlides structure
PRINT 'HeroSlides columns:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'HeroSlides'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- Check SuccessStories structure  
PRINT 'SuccessStories columns:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SuccessStories'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- Step 3: Verify no legacy columns exist
PRINT '=== VERIFYING LEGACY COLUMNS REMOVED ===';

DECLARE @LegacyColumns TABLE (TableName NVARCHAR(50), ColumnName NVARCHAR(50));

-- Check for legacy columns in HeroSlides
INSERT INTO @LegacyColumns
SELECT 'HeroSlides', COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'HeroSlides'
AND COLUMN_NAME IN ('Badge', 'PrimaryActionLabel', 'PrimaryActionLink', 'SecondaryActionLabel', 
                   'SecondaryActionLink', 'StatsJson', 'IsPermanent', 'ExpiresAt', 'ButtonText', 'LinkUrl');

-- Check for legacy columns in SuccessStories
INSERT INTO @LegacyColumns
SELECT 'SuccessStories', COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SuccessStories'
AND COLUMN_NAME IN ('Badge', 'StudentName', 'CourseName', 'Subtitle', 'Type', 'StatsJson', 
                   'Duration', 'IsPermanent', 'ExpiresAt', 'LinkUrl', 'CourseId');

IF EXISTS (SELECT * FROM @LegacyColumns)
BEGIN
    PRINT '✗ WARNING: Legacy columns still exist:';
    SELECT TableName, ColumnName FROM @LegacyColumns;
    PRINT 'Migration may not have completed successfully.';
END
ELSE
    PRINT '✓ All legacy columns have been successfully removed';

PRINT '';

-- Step 4: Final verification
PRINT '=== FINAL VERIFICATION ===';

DECLARE @HeroSlideCount INT, @SuccessStoryCount INT;
SELECT @HeroSlideCount = COUNT(*) FROM HeroSlides;
SELECT @SuccessStoryCount = COUNT(*) FROM SuccessStories;

PRINT 'Current data counts:';
PRINT 'HeroSlides: ' + CAST(@HeroSlideCount AS VARCHAR(10)) + ' records';
PRINT 'SuccessStories: ' + CAST(@SuccessStoryCount AS VARCHAR(10)) + ' records';

PRINT '';
PRINT '✓ Legacy slider data cleanup completed successfully';
PRINT '✓ Backup tables removed';
PRINT '✓ Simplified slider structure verified';

COMMIT TRANSACTION;

PRINT '';
PRINT 'Cleanup completed. The slider system is now fully simplified.';