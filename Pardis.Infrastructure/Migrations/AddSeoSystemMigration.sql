-- SEO System Migration
-- Adds comprehensive SEO support to existing entities

-- Add SEO columns to Categories table
ALTER TABLE Categories ADD COLUMN SeoTitle NVARCHAR(60) NULL;
ALTER TABLE Categories ADD COLUMN SeoDescription NVARCHAR(160) NULL;
ALTER TABLE Categories ADD COLUMN SeoKeywords NVARCHAR(255) NULL;
ALTER TABLE Categories ADD COLUMN CanonicalUrl NVARCHAR(500) NULL;
ALTER TABLE Categories ADD COLUMN RobotsIndex BIT NOT NULL DEFAULT 1;
ALTER TABLE Categories ADD COLUMN RobotsFollow BIT NOT NULL DEFAULT 1;
ALTER TABLE Categories ADD COLUMN OpenGraphTitle NVARCHAR(60) NULL;
ALTER TABLE Categories ADD COLUMN OpenGraphDescription NVARCHAR(160) NULL;
ALTER TABLE Categories ADD COLUMN OpenGraphImage NVARCHAR(500) NULL;
ALTER TABLE Categories ADD COLUMN TwitterTitle NVARCHAR(60) NULL;
ALTER TABLE Categories ADD COLUMN TwitterDescription NVARCHAR(160) NULL;
ALTER TABLE Categories ADD COLUMN TwitterImage NVARCHAR(500) NULL;
ALTER TABLE Categories ADD COLUMN JsonLdSchemas NVARCHAR(MAX) NULL;

-- Add SEO columns to Courses table
ALTER TABLE Courses ADD COLUMN SeoTitle NVARCHAR(60) NULL;
ALTER TABLE Courses ADD COLUMN SeoDescription NVARCHAR(160) NULL;
ALTER TABLE Courses ADD COLUMN SeoKeywords NVARCHAR(255) NULL;
ALTER TABLE Courses ADD COLUMN CanonicalUrl NVARCHAR(500) NULL;
ALTER TABLE Courses ADD COLUMN RobotsIndex BIT NOT NULL DEFAULT 1;
ALTER TABLE Courses ADD COLUMN RobotsFollow BIT NOT NULL DEFAULT 1;
ALTER TABLE Courses ADD COLUMN OpenGraphTitle NVARCHAR(60) NULL;
ALTER TABLE Courses ADD COLUMN OpenGraphDescription NVARCHAR(160) NULL;
ALTER TABLE Courses ADD COLUMN OpenGraphImage NVARCHAR(500) NULL;
ALTER TABLE Courses ADD COLUMN TwitterTitle NVARCHAR(60) NULL;
ALTER TABLE Courses ADD COLUMN TwitterDescription NVARCHAR(160) NULL;
ALTER TABLE Courses ADD COLUMN TwitterImage NVARCHAR(500) NULL;
ALTER TABLE Courses ADD COLUMN JsonLdSchemas NVARCHAR(MAX) NULL;

-- Create Pages table for static/dynamic pages
CREATE TABLE Pages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Slug NVARCHAR(255) NOT NULL UNIQUE,
    Title NVARCHAR(255) NOT NULL,
    Content NVARCHAR(MAX) NULL,
    PageType NVARCHAR(50) NOT NULL, -- Static, Landing, Campaign
    IsPublished BIT NOT NULL DEFAULT 1,
    SeoTitle NVARCHAR(60) NULL,
    SeoDescription NVARCHAR(160) NULL,
    SeoKeywords NVARCHAR(255) NULL,
    CanonicalUrl NVARCHAR(500) NULL,
    RobotsIndex BIT NOT NULL DEFAULT 1,
    RobotsFollow BIT NOT NULL DEFAULT 1,
    OpenGraphTitle NVARCHAR(60) NULL,
    OpenGraphDescription NVARCHAR(160) NULL,
    OpenGraphImage NVARCHAR(500) NULL,
    TwitterTitle NVARCHAR(60) NULL,
    TwitterDescription NVARCHAR(160) NULL,
    TwitterImage NVARCHAR(500) NULL,
    JsonLdSchemas NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Create indexes for performance
CREATE INDEX IX_Categories_Slug ON Categories(Slug);
CREATE INDEX IX_Courses_Slug ON Courses(Slug);
CREATE INDEX IX_Pages_Slug ON Pages(Slug);
CREATE INDEX IX_Pages_PageType ON Pages(PageType);
CREATE INDEX IX_Pages_IsPublished ON Pages(IsPublished);