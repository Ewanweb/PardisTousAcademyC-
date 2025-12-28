-- Test Script for Migration Logic
-- This script tests the migration logic without actually running the full migration

PRINT 'Testing migration logic for slider simplification...';

-- Create temporary test tables with legacy structure
CREATE TABLE #TestHeroSlides (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    ImageUrl NVARCHAR(500) NOT NULL,
    ButtonText NVARCHAR(100),
    LinkUrl NVARCHAR(500),
    PrimaryActionLabel NVARCHAR(100),
    PrimaryActionLink NVARCHAR(500),
    SecondaryActionLabel NVARCHAR(100),
    SecondaryActionLink NVARCHAR(500),
    Badge NVARCHAR(100),
    StatsJson NVARCHAR(MAX),
    [Order] INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    IsPermanent BIT NOT NULL DEFAULT 0,
    ExpiresAt DATETIME2,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    -- New simplified columns
    ActionLabel NVARCHAR(100),
    ActionLink NVARCHAR(500)
);

CREATE TABLE #TestSuccessStories (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    ImageUrl NVARCHAR(500) NOT NULL,
    LinkUrl NVARCHAR(500),
    ActionLabel NVARCHAR(100),
    ActionLink NVARCHAR(500),
    StudentName NVARCHAR(100),
    CourseName NVARCHAR(200),
    Subtitle NVARCHAR(100),
    Badge NVARCHAR(100),
    Type NVARCHAR(50) NOT NULL DEFAULT 'success',
    StatsJson NVARCHAR(MAX),
    Duration INT,
    [Order] INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    IsPermanent BIT NOT NULL DEFAULT 0,
    ExpiresAt DATETIME2,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);

-- Insert test data
INSERT INTO #TestHeroSlides (
    Id, Title, Description, ImageUrl, ButtonText, LinkUrl, PrimaryActionLabel, PrimaryActionLink,
    Badge, StatsJson, [Order], IsActive, CreatedByUserId, CreatedAt, UpdatedAt
) VALUES 
(NEWID(), 'Test Hero 1', 'Description 1', 'https://example.com/1.jpg', 'Button 1', 'https://link1.com', NULL, NULL, 'Badge 1', '{"stat": "value"}', 1, 1, NEWID(), GETDATE(), GETDATE()),
(NEWID(), 'Test Hero 2', 'Description 2', 'https://example.com/2.jpg', NULL, NULL, 'Primary Action', 'https://primary.com', 'Badge 2', NULL, 2, 1, NEWID(), GETDATE(), GETDATE()),
(NEWID(), 'Test Hero 3', 'Description 3', 'https://example.com/3.jpg', 'Button 3', 'https://link3.com', 'Primary Override', 'https://override.com', NULL, NULL, 3, 0, NEWID(), GETDATE(), GETDATE());

INSERT INTO #TestSuccessStories (
    Id, Title, Description, ImageUrl, LinkUrl, ActionLabel, ActionLink, StudentName, CourseName,
    Type, [Order], IsActive, CreatedByUserId, CreatedAt, UpdatedAt
) VALUES 
(NEWID(), 'Success 1', 'Story 1', 'https://example.com/s1.jpg', 'https://story1.com', NULL, NULL, 'Student 1', 'Course 1', 'success', 1, 1, NEWID(), GETDATE(), GETDATE()),
(NEWID(), 'Success 2', 'Story 2', 'https://example.com/s2.jpg', 'https://story2.com', 'Custom Action', 'https://custom.com', 'Student 2', 'Course 2', 'video', 2, 1, NEWID(), GETDATE(), GETDATE()),
(NEWID(), 'Success 3', 'Story 3', 'https://example.com/s3.jpg', NULL, 'Existing Action', 'https://existing.com', 'Student 3', 'Course 3', 'success', 3, 0, NEWID(), GETDATE(), GETDATE());

PRINT 'Test data inserted. Testing migration logic...';

-- Test HeroSlides migration logic
UPDATE #TestHeroSlides 
SET ActionLabel = CASE 
    WHEN PrimaryActionLabel IS NOT NULL AND LTRIM(RTRIM(PrimaryActionLabel)) != '' THEN PrimaryActionLabel
    WHEN ButtonText IS NOT NULL AND LTRIM(RTRIM(ButtonText)) != '' THEN ButtonText
    ELSE NULL
END,
ActionLink = CASE 
    WHEN PrimaryActionLink IS NOT NULL AND LTRIM(RTRIM(PrimaryActionLink)) != '' THEN PrimaryActionLink
    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN LinkUrl
    ELSE NULL
END;

-- Test SuccessStories migration logic
UPDATE #TestSuccessStories 
SET ActionLabel = CASE 
    WHEN ActionLabel IS NOT NULL AND LTRIM(RTRIM(ActionLabel)) != '' THEN ActionLabel
    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN N'مشاهده'
    ELSE NULL
END,
ActionLink = CASE 
    WHEN ActionLink IS NOT NULL AND LTRIM(RTRIM(ActionLink)) != '' THEN ActionLink
    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN LinkUrl
    ELSE NULL
END;

-- Verify results
PRINT '';
PRINT '=== MIGRATION LOGIC TEST RESULTS ===';
PRINT '';
PRINT 'HeroSlides migration results:';
SELECT 
    Title,
    ButtonText as OriginalButton,
    LinkUrl as OriginalLink,
    PrimaryActionLabel as OriginalPrimary,
    PrimaryActionLink as OriginalPrimaryLink,
    ActionLabel as MigratedLabel,
    ActionLink as MigratedLink,
    CASE 
        WHEN (PrimaryActionLabel IS NOT NULL AND ActionLabel = PrimaryActionLabel) THEN 'Primary Used'
        WHEN (ButtonText IS NOT NULL AND ActionLabel = ButtonText) THEN 'Button Used'
        WHEN (ActionLabel IS NULL AND PrimaryActionLabel IS NULL AND ButtonText IS NULL) THEN 'Correctly Null'
        ELSE 'ERROR'
    END as MigrationStatus
FROM #TestHeroSlides
ORDER BY [Order];

PRINT '';
PRINT 'SuccessStories migration results:';
SELECT 
    Title,
    LinkUrl as OriginalLink,
    ActionLabel as MigratedLabel,
    ActionLink as MigratedLink,
    CASE 
        WHEN (ActionLabel = N'مشاهده' AND LinkUrl IS NOT NULL) THEN 'Default Applied'
        WHEN (ActionLabel = 'Custom Action') THEN 'Existing Preserved'
        WHEN (ActionLabel = 'Existing Action') THEN 'Existing Preserved'
        WHEN (ActionLabel IS NULL AND LinkUrl IS NULL) THEN 'Correctly Null'
        ELSE 'ERROR'
    END as MigrationStatus
FROM #TestSuccessStories
ORDER BY [Order];

-- Verify data integrity
PRINT '';
PRINT '=== DATA INTEGRITY VERIFICATION ===';

DECLARE @HeroErrors INT = 0, @SuccessErrors INT = 0;

-- Check HeroSlides integrity
SELECT @HeroErrors = COUNT(*)
FROM #TestHeroSlides
WHERE 
    (PrimaryActionLabel IS NOT NULL AND ActionLabel != PrimaryActionLabel)
    OR (PrimaryActionLabel IS NULL AND ButtonText IS NOT NULL AND ActionLabel != ButtonText)
    OR (PrimaryActionLabel IS NULL AND ButtonText IS NULL AND ActionLabel IS NOT NULL);

-- Check SuccessStories integrity  
SELECT @SuccessErrors = COUNT(*)
FROM #TestSuccessStories
WHERE 
    (ActionLabel NOT IN (N'مشاهده', 'Custom Action', 'Existing Action') AND ActionLabel IS NOT NULL)
    OR (LinkUrl IS NOT NULL AND ActionLabel IS NULL);

PRINT 'HeroSlides migration errors: ' + CAST(@HeroErrors AS VARCHAR(10));
PRINT 'SuccessStories migration errors: ' + CAST(@SuccessErrors AS VARCHAR(10));

IF @HeroErrors = 0 AND @SuccessErrors = 0
    PRINT '✓ All migration logic tests passed successfully!';
ELSE
    PRINT '✗ Migration logic has errors that need to be fixed.';

-- Clean up
DROP TABLE #TestHeroSlides;
DROP TABLE #TestSuccessStories;

PRINT '';
PRINT 'Migration logic test completed.';