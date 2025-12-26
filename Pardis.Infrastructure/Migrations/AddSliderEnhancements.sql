-- Migration: Add Slider Enhancements
-- This migration adds new columns to support enhanced slider functionality

BEGIN TRANSACTION;

-- Add new columns to HeroSlides table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HeroSlides]') AND name = 'Badge')
    ALTER TABLE HeroSlides ADD Badge NVARCHAR(100) NULL;

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

-- Add new columns to SuccessStories table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Subtitle')
    ALTER TABLE SuccessStories ADD Subtitle NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Badge')
    ALTER TABLE SuccessStories ADD Badge NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Type')
    ALTER TABLE SuccessStories ADD Type NVARCHAR(50) NOT NULL DEFAULT 'success';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'ActionLabel')
    ALTER TABLE SuccessStories ADD ActionLabel NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'ActionLink')
    ALTER TABLE SuccessStories ADD ActionLink NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'StatsJson')
    ALTER TABLE SuccessStories ADD StatsJson NVARCHAR(MAX) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SuccessStories]') AND name = 'Duration')
    ALTER TABLE SuccessStories ADD Duration INT NULL;

-- Migrate existing data to new columns
UPDATE HeroSlides 
SET PrimaryActionLabel = ButtonText, 
    PrimaryActionLink = LinkUrl 
WHERE PrimaryActionLabel IS NULL AND (ButtonText IS NOT NULL OR LinkUrl IS NOT NULL);

UPDATE SuccessStories 
SET ActionLabel = 'مشاهده', 
    ActionLink = LinkUrl 
WHERE ActionLabel IS NULL AND LinkUrl IS NOT NULL;

-- Create indexes for better performance
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

-- Insert sample data for testing (optional)
-- This data matches the frontend's expected structure

-- Sample Hero Slides
INSERT INTO HeroSlides (
    Id, Title, Description, ImageUrl, Badge, 
    PrimaryActionLabel, PrimaryActionLink, 
    SecondaryActionLabel, SecondaryActionLink,
    StatsJson, [Order], IsActive, IsPermanent, ExpiresAt,
    CreatedByUserId, CreatedAt, UpdatedAt
) VALUES 
(
    NEWID(),
    'آموزش برنامه‌نویسی از صفر تا صد',
    'با مسیرهای یادگیری پروژه‌محور، مهارت‌های کدنویسی خود را حرفه‌ای کنید',
    'https://images.unsplash.com/photo-1517077304055-6e89abbf09b0?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80',
    'محبوب‌ترین دوره‌ها',
    'شروع یادگیری',
    '/courses',
    'مشاهده نمونه',
    'https://www.youtube.com/watch?v=dQw4w9WgXcQ',
    '[{"icon":"Users","value":"2000+","label":"دانشجو"},{"icon":"Star","value":"4.9","label":"امتیاز"},{"icon":"Clock","value":"24/7","label":"پشتیبانی"}]',
    1, 1, 1, NULL,
    '00000000-0000-0000-0000-000000000000', GETUTCDATE(), GETUTCDATE()
),
(
    NEWID(),
    'طراحی UI/UX حرفه‌ای',
    'از اصول طراحی تا ساخت پروتوتایپ‌های تعاملی با ابزارهای مدرن',
    'https://images.unsplash.com/photo-1561070791-2526d30994b5?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80',
    'جدیدترین دوره',
    'ثبت‌نام کنید',
    '/courses?category=design',
    'نمونه کارها',
    'https://dribbble.com/shots/popular/web-design',
    '[{"icon":"Users","value":"1500+","label":"دانشجو"},{"icon":"Star","value":"4.8","label":"امتیاز"},{"icon":"BookOpen","value":"12","label":"پروژه"}]',
    2, 1, 0, DATEADD(hour, 21, GETUTCDATE()),
    '00000000-0000-0000-0000-000000000000', GETUTCDATE(), GETUTCDATE()
);

-- Sample Success Stories
INSERT INTO SuccessStories (
    Id, Title, Subtitle, Description, ImageUrl, Badge, Type,
    StudentName, CourseName, ActionLabel, ActionLink,
    StatsJson, Duration, [Order], IsActive, IsPermanent, ExpiresAt,
    CreatedByUserId, CreatedAt, UpdatedAt
) VALUES 
(
    NEWID(),
    'از صفر تا برنامه‌نویس',
    'سارا احمدی',
    'بعد از گذراندن دوره React، الان توی یک شرکت بین‌المللی کار می‌کنم و درآمد ماهانه‌ام 3 برابر شده!',
    'https://images.unsplash.com/photo-1494790108755-2616b612b786?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80',
    'موفقیت',
    'success',
    'سارا احمدی',
    'React Development',
    'مشاهده پروفایل',
    '/profile/sara-ahmadi',
    '[{"value":"6 ماه","label":"مدت یادگیری"},{"value":"15M","label":"حقوق فعلی"}]',
    6000, 1, 1, 1, NULL,
    '00000000-0000-0000-0000-000000000000', GETUTCDATE(), GETUTCDATE()
),
(
    NEWID(),
    'طراح UI/UX شدم',
    'محمد رضایی',
    'دوره طراحی رابط کاربری زندگی‌ام رو عوض کرد. الان فریلنسر هستم و پروژه‌های بین‌المللی می‌گیرم.',
    'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80',
    'طراح',
    'success',
    'محمد رضایی',
    'UI/UX Design',
    'نمونه کارها',
    'https://behance.net/mohammad-rezaei',
    '[{"value":"4 ماه","label":"مدت یادگیری"},{"value":"20+","label":"پروژه موفق"}]',
    5500, 2, 1, 0, DATEADD(hour, 22, GETUTCDATE()),
    '00000000-0000-0000-0000-000000000000', GETUTCDATE(), GETUTCDATE()
);

-- Add constraints for data integrity
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_HeroSlides_Order')
    ALTER TABLE HeroSlides ADD CONSTRAINT CK_HeroSlides_Order CHECK ([Order] >= 0);

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Order')
    ALTER TABLE SuccessStories ADD CONSTRAINT CK_SuccessStories_Order CHECK ([Order] >= 0);

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Duration')
    ALTER TABLE SuccessStories ADD CONSTRAINT CK_SuccessStories_Duration CHECK (Duration >= 1000 AND Duration <= 30000);

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SuccessStories_Type')
    ALTER TABLE SuccessStories ADD CONSTRAINT CK_SuccessStories_Type CHECK (Type IN ('success', 'video'));

COMMIT TRANSACTION;

PRINT 'Migration completed successfully: Enhanced slider support added';