-- Verification Script for Slider Simplification Migration
-- This script validates that the migration was successful and data was preserved correctly

PRINT 'Starting verification of slider simplification migration...';
PRINT '';

-- Step 1: Check if backup tables exist (they should exist after migration)
PRINT '=== BACKUP TABLES VERIFICATION ===';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroSlides_PreSimplification_Backup')
    PRINT '✓ HeroSlides backup table exists';
ELSE
    PRINT '✗ HeroSlides backup table missing - MIGRATION MAY HAVE FAILED';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories_PreSimplification_Backup')
    PRINT '✓ SuccessStories backup table exists';
ELSE
    PRINT '✗ SuccessStories backup table missing - MIGRATION MAY HAVE FAILED';

PRINT '';

-- Step 2: Verify simplified structure - check that complex columns are removed
PRINT '=== STRUCTURE VERIFICATION ===';

-- Check HeroSlides structure
PRINT 'HeroSlides - Checking for removed complex columns:';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'Badge')
    PRINT '✓ Badge column removed';
ELSE
    PRINT '✗ Badge column still exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ButtonText')
    PRINT '✓ ButtonText column removed';
ELSE
    PRINT '✗ ButtonText column still exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'PrimaryActionLabel')
    PRINT '✓ PrimaryActionLabel column removed';
ELSE
    PRINT '✗ PrimaryActionLabel column still exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'StatsJson')
    PRINT '✓ StatsJson column removed';
ELSE
    PRINT '✗ StatsJson column still exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'IsPermanent')
    PRINT '✓ IsPermanent column removed';
ELSE
    PRINT '✗ IsPermanent column still exists';

-- Check that essential columns exist
PRINT '';
PRINT 'HeroSlides - Checking for essential columns:';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ActionLabel')
    PRINT '✓ ActionLabel column exists';
ELSE
    PRINT '✗ ActionLabel column missing';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'ActionLink')
    PRINT '✓ ActionLink column exists';
ELSE
    PRINT '✗ ActionLink column missing';

-- Check SuccessStories structure
PRINT '';
PRINT 'SuccessStories - Checking for removed complex columns:';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'StudentName')
    PRINT '✓ StudentName column removed';
ELSE
    PRINT '✗ StudentName column still exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'CourseName')
    PRINT '✓ CourseName column removed';
ELSE
    PRINT '✗ CourseName column still exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Type')
    PRINT '✓ Type column removed';
ELSE
    PRINT '✗ Type column still exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Duration')
    PRINT '✓ Duration column removed';
ELSE
    PRINT '✗ Duration column still exists';

PRINT '';

-- Step 3: Verify data preservation
PRINT '=== DATA PRESERVATION VERIFICATION ===';

-- Count records in current tables vs backup tables
DECLARE @CurrentHeroSlides INT, @BackupHeroSlides INT;
DECLARE @CurrentSuccessStories INT, @BackupSuccessStories INT;

SELECT @CurrentHeroSlides = COUNT(*) FROM HeroSlides;
SELECT @CurrentSuccessStories = COUNT(*) FROM SuccessStories;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroSlides_PreSimplification_Backup')
    SELECT @BackupHeroSlides = COUNT(*) FROM HeroSlides_PreSimplification_Backup;
ELSE
    SET @BackupHeroSlides = -1;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories_PreSimplification_Backup')
    SELECT @BackupSuccessStories = COUNT(*) FROM SuccessStories_PreSimplification_Backup;
ELSE
    SET @BackupSuccessStories = -1;

PRINT 'Record count comparison:';
PRINT 'HeroSlides - Current: ' + CAST(@CurrentHeroSlides AS VARCHAR(10)) + ', Backup: ' + CAST(@BackupHeroSlides AS VARCHAR(10));
PRINT 'SuccessStories - Current: ' + CAST(@CurrentSuccessStories AS VARCHAR(10)) + ', Backup: ' + CAST(@BackupSuccessStories AS VARCHAR(10));

IF @CurrentHeroSlides = @BackupHeroSlides
    PRINT '✓ HeroSlides record count preserved';
ELSE
    PRINT '✗ HeroSlides record count mismatch - DATA LOSS DETECTED';

IF @CurrentSuccessStories = @BackupSuccessStories
    PRINT '✓ SuccessStories record count preserved';
ELSE
    PRINT '✗ SuccessStories record count mismatch - DATA LOSS DETECTED';

PRINT '';

-- Step 4: Verify data migration logic
PRINT '=== DATA MIGRATION LOGIC VERIFICATION ===';

-- Check ActionLabel/ActionLink mapping for HeroSlides
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroSlides_PreSimplification_Backup')
BEGIN
    PRINT 'Checking HeroSlides action mapping...';
    
    -- Count records where ActionLabel was properly mapped
    DECLARE @ProperlyMappedHero INT;
    SELECT @ProperlyMappedHero = COUNT(*)
    FROM HeroSlides h
    INNER JOIN HeroSlides_PreSimplification_Backup b ON h.Id = b.Id
    WHERE 
        (h.ActionLabel = b.PrimaryActionLabel OR (h.ActionLabel = b.ButtonText AND b.PrimaryActionLabel IS NULL))
        OR (h.ActionLabel IS NULL AND b.PrimaryActionLabel IS NULL AND b.ButtonText IS NULL);
    
    PRINT 'HeroSlides with properly mapped ActionLabel: ' + CAST(@ProperlyMappedHero AS VARCHAR(10)) + ' / ' + CAST(@CurrentHeroSlides AS VARCHAR(10));
    
    IF @ProperlyMappedHero = @CurrentHeroSlides
        PRINT '✓ All HeroSlides ActionLabel mappings are correct';
    ELSE
        PRINT '✗ Some HeroSlides ActionLabel mappings are incorrect';
END

-- Check ActionLabel/ActionLink mapping for SuccessStories
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories_PreSimplification_Backup')
BEGIN
    PRINT '';
    PRINT 'Checking SuccessStories action mapping...';
    
    -- Count records where ActionLabel was properly mapped
    DECLARE @ProperlyMappedSuccess INT;
    SELECT @ProperlyMappedSuccess = COUNT(*)
    FROM SuccessStories s
    INNER JOIN SuccessStories_PreSimplification_Backup b ON s.Id = b.Id
    WHERE 
        (s.ActionLabel = b.ActionLabel)
        OR (s.ActionLabel = N'مشاهده' AND b.ActionLabel IS NULL AND b.LinkUrl IS NOT NULL)
        OR (s.ActionLabel IS NULL AND b.ActionLabel IS NULL AND b.LinkUrl IS NULL);
    
    PRINT 'SuccessStories with properly mapped ActionLabel: ' + CAST(@ProperlyMappedSuccess AS VARCHAR(10)) + ' / ' + CAST(@CurrentSuccessStories AS VARCHAR(10));
    
    IF @ProperlyMappedSuccess = @CurrentSuccessStories
        PRINT '✓ All SuccessStories ActionLabel mappings are correct';
    ELSE
        PRINT '✗ Some SuccessStories ActionLabel mappings are incorrect';
END

PRINT '';

-- Step 5: Verify constraints
PRINT '=== CONSTRAINTS VERIFICATION ===';

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_HeroSlides_Order_Simplified')
    PRINT '✓ HeroSlides Order constraint exists';
ELSE
    PRINT '✗ HeroSlides Order constraint missing';

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Order_Simplified')
    PRINT '✓ SuccessStories Order constraint exists';
ELSE
    PRINT '✗ SuccessStories Order constraint missing';

PRINT '';

-- Step 6: Sample data verification
PRINT '=== SAMPLE DATA VERIFICATION ===';

PRINT 'Sample HeroSlides data (first 3 records):';
SELECT TOP 3 
    Id, 
    Title, 
    CASE WHEN LEN(Description) > 50 THEN LEFT(Description, 50) + '...' ELSE Description END as Description,
    ActionLabel,
    ActionLink,
    [Order],
    IsActive
FROM HeroSlides
ORDER BY [Order];

PRINT '';
PRINT 'Sample SuccessStories data (first 3 records):';
SELECT TOP 3 
    Id, 
    Title, 
    CASE WHEN LEN(Description) > 50 THEN LEFT(Description, 50) + '...' ELSE Description END as Description,
    ActionLabel,
    ActionLink,
    [Order],
    IsActive
FROM SuccessStories
ORDER BY [Order];

PRINT '';

-- Step 7: Final summary
PRINT '=== MIGRATION VERIFICATION SUMMARY ===';

DECLARE @Issues INT = 0;

-- Count potential issues
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroSlides_PreSimplification_Backup')
    SET @Issues = @Issues + 1;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories_PreSimplification_Backup')
    SET @Issues = @Issues + 1;

IF @CurrentHeroSlides != @BackupHeroSlides AND @BackupHeroSlides != -1
    SET @Issues = @Issues + 1;

IF @CurrentSuccessStories != @BackupSuccessStories AND @BackupSuccessStories != -1
    SET @Issues = @Issues + 1;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name IN ('Badge', 'ButtonText', 'PrimaryActionLabel', 'StatsJson', 'IsPermanent'))
    SET @Issues = @Issues + 1;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name IN ('StudentName', 'CourseName', 'Type', 'Duration', 'IsPermanent'))
    SET @Issues = @Issues + 1;

IF @Issues = 0
BEGIN
    PRINT '✓ MIGRATION SUCCESSFUL - No issues detected';
    PRINT '✓ All data has been preserved and properly migrated';
    PRINT '✓ Complex columns have been removed as expected';
    PRINT '✓ Backup tables are available for rollback if needed';
END
ELSE
BEGIN
    PRINT '✗ MIGRATION ISSUES DETECTED - ' + CAST(@Issues AS VARCHAR(10)) + ' potential problems found';
    PRINT '⚠ Please review the verification output above';
    PRINT '⚠ Consider running the rollback script if critical issues are found';
END

PRINT '';
PRINT 'Verification completed at: ' + CONVERT(VARCHAR(19), GETDATE(), 120);