# Slider Simplification Migration

This directory contains the migration scripts for simplifying the slider system by removing complex fields and features while maintaining core functionality.

## Migration Overview

The migration transforms the slider system from a complex structure with multiple action buttons, stats, badges, and expiration logic to a simplified structure with only essential fields.

### What Changes

**HeroSlides Table:**

- **Removed Fields:** Badge, ButtonText, LinkUrl, PrimaryActionLabel, PrimaryActionLink, SecondaryActionLabel, SecondaryActionLink, StatsJson, IsPermanent, ExpiresAt
- **Added Fields:** ActionLabel, ActionLink (consolidated from legacy fields)
- **Preserved Fields:** Title, Description, ImageUrl, Order, IsActive, CreatedByUserId, CreatedAt, UpdatedAt

**SuccessStories Table:**

- **Removed Fields:** Badge, LinkUrl, StudentName, CourseName, Subtitle, Type, StatsJson, Duration, IsPermanent, ExpiresAt, CourseId
- **Preserved Fields:** Title, Description, ImageUrl, ActionLabel, ActionLink, Order, IsActive, CreatedByUserId, CreatedAt, UpdatedAt
- **Modified:** Description column length reduced from 1000 to 500 characters

## Migration Files

### 1. `20251228120000_SimplifySliderStructure.cs`

The main Entity Framework migration file that:

- Creates backup tables for rollback safety
- Migrates existing data to simplified structure
- Removes complex columns
- Sets up new constraints
- Maps legacy fields to new simplified fields

### 2. `RollbackSliderSimplification.sql`

Standalone SQL script for rolling back the migration:

- Restores complex column structure
- Recovers data from backup tables
- Re-establishes original constraints and indexes
- Provides verification of rollback success

### 3. `VerifySliderSimplification.sql`

Comprehensive verification script that:

- Checks backup table existence
- Verifies column structure changes
- Validates data preservation
- Tests migration logic correctness
- Provides detailed status report

### 4. `TestMigrationLogic.sql`

Test script for validating migration logic:

- Creates temporary test tables
- Inserts sample data
- Tests field mapping logic
- Verifies data integrity
- Safe to run without affecting real data

## Data Migration Logic

### HeroSlides Action Mapping

```sql
ActionLabel = CASE
    WHEN PrimaryActionLabel IS NOT NULL AND LTRIM(RTRIM(PrimaryActionLabel)) != '' THEN PrimaryActionLabel
    WHEN ButtonText IS NOT NULL AND LTRIM(RTRIM(ButtonText)) != '' THEN ButtonText
    ELSE NULL
END

ActionLink = CASE
    WHEN PrimaryActionLink IS NOT NULL AND LTRIM(RTRIM(PrimaryActionLink)) != '' THEN PrimaryActionLink
    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN LinkUrl
    ELSE NULL
END
```

### SuccessStories Action Mapping

```sql
ActionLabel = CASE
    WHEN ActionLabel IS NOT NULL AND LTRIM(RTRIM(ActionLabel)) != '' THEN ActionLabel
    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN N'مشاهده'
    ELSE NULL
END

ActionLink = CASE
    WHEN ActionLink IS NOT NULL AND LTRIM(RTRIM(ActionLink)) != '' THEN ActionLink
    WHEN LinkUrl IS NOT NULL AND LTRIM(RTRIM(LinkUrl)) != '' THEN LinkUrl
    ELSE NULL
END
```

## Running the Migration

### Prerequisites

- Backup your database before running the migration
- Ensure no active connections are modifying slider data
- Verify you have sufficient disk space for backup tables

### Step 1: Test Migration Logic (Optional but Recommended)

```sql
-- Run the test script to verify logic
EXEC sqlcmd -S your_server -d your_database -i TestMigrationLogic.sql
```

### Step 2: Run Entity Framework Migration

```bash
# Navigate to the project directory
cd Backend/PardisTousAcademy

# Run the migration
dotnet ef database update --project Pardis.Infrastructure --startup-project Endpoints/Api
```

### Step 3: Verify Migration Success

```sql
-- Run the verification script
EXEC sqlcmd -S your_server -d your_database -i VerifySliderSimplification.sql
```

## Rollback Procedure

If you need to rollback the migration:

### Option 1: Entity Framework Rollback

```bash
# Rollback to the previous migration
dotnet ef database update 20251227180455_sa --project Pardis.Infrastructure --startup-project Endpoints/Api
```

### Option 2: Manual SQL Rollback

```sql
-- Run the rollback script
EXEC sqlcmd -S your_server -d your_database -i RollbackSliderSimplification.sql
```

## Safety Features

1. **Backup Tables:** The migration creates backup tables (`HeroSlides_PreSimplification_Backup`, `SuccessStories_PreSimplification_Backup`) containing all original data.

2. **Data Preservation:** All essential fields (Title, Description, ImageUrl) are preserved during migration.

3. **Rollback Capability:** Both EF rollback and manual SQL rollback options are available.

4. **Verification:** Comprehensive verification scripts ensure migration success.

5. **Test Scripts:** Safe testing of migration logic without affecting real data.

## Post-Migration Tasks

After successful migration:

1. **Update Application Code:** Ensure all application handlers, controllers, and DTOs are updated to use the simplified structure.

2. **Update Frontend:** Modify React components to work with the simplified data structure.

3. **Clean Up:** After confirming everything works correctly, you may optionally remove backup tables:

   ```sql
   DROP TABLE HeroSlides_PreSimplification_Backup;
   DROP TABLE SuccessStories_PreSimplification_Backup;
   ```

4. **Update Documentation:** Update API documentation to reflect the simplified structure.

## Troubleshooting

### Common Issues

1. **Migration Fails:** Check for foreign key constraints or active connections. Ensure database has sufficient space.

2. **Data Loss Detected:** Run the verification script to identify specific issues. Use rollback if necessary.

3. **Application Errors:** Update application code to use new simplified field names (ActionLabel/ActionLink instead of ButtonText/LinkUrl).

### Support

- Check verification script output for detailed error information
- Review backup tables to confirm data preservation
- Use test script to validate migration logic before applying to production

## Requirements Validation

This migration satisfies the following requirements:

- **8.1:** Preserves existing Title, Description, ImageUrl for HeroSlides
- **8.2:** Preserves existing Title, Description, ImageUrl for SuccessStories
- **8.3:** Maps legacy ButtonText to ActionLabel and LinkUrl to ActionLink
- **8.4:** Sets default values for new required fields (Order, IsActive)
- **8.5:** Provides rollback capability through backup tables and scripts
