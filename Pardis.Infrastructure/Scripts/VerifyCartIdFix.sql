-- Verification script for CartId NULL fix
-- Run this after applying the migration to verify everything is working correctly

-- 1. Check that no NULL CartId values exist
SELECT 'NULL CartId Check' AS CheckType, COUNT(*) AS Count
FROM dbo.Orders 
WHERE CartId IS NULL;

-- 2. Check that no Guid.Empty CartId values exist
SELECT 'Empty GUID CartId Check' AS CheckType, COUNT(*) AS Count
FROM dbo.Orders 
WHERE CartId = '00000000-0000-0000-0000-000000000000';

-- 3. Verify unique index exists and is working
SELECT 
    'Index Exists Check' AS CheckType,
    CASE WHEN EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'IX_Orders_UserId_CartId_Active'
          AND object_id = OBJECT_ID(N'dbo.Orders')
    ) THEN 'EXISTS' ELSE 'MISSING' END AS Status;

-- 4. Check for potential duplicate active orders (should be 0)
SELECT 'Duplicate Active Orders Check' AS CheckType, COUNT(*) AS Count
FROM (
    SELECT UserId, CartId, COUNT(*) as OrderCount
    FROM dbo.Orders
    WHERE Status IN (0, 1) -- Draft=0, PendingPayment=1
    GROUP BY UserId, CartId
    HAVING COUNT(*) > 1
) duplicates;

-- 5. Check constraint exists
SELECT 
    'Check Constraint Exists' AS CheckType,
    CASE WHEN EXISTS (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_Orders_CartId_NotEmpty'
          AND parent_object_id = OBJECT_ID(N'dbo.Orders')
    ) THEN 'EXISTS' ELSE 'MISSING' END AS Status;

-- 6. Sample data verification - show some orders with their CartIds
SELECT TOP 10
    Id,
    UserId,
    CartId,
    OrderNumber,
    Status,
    CreatedAt
FROM dbo.Orders
ORDER BY CreatedAt DESC;

-- 7. Test the unique constraint (this should fail if run twice with same values)
-- Uncomment to test:
/*
BEGIN TRY
    INSERT INTO dbo.Orders (Id, UserId, CartId, OrderNumber, TotalAmount, Currency, Status, CartSnapshot, CreatedAt, UpdatedAt)
    VALUES (
        NEWID(),
        'test-user-constraint',
        NEWID(),
        'TEST-CONSTRAINT-001',
        100000,
        'IRR',
        0, -- Draft
        '[]',
        GETUTCDATE(),
        GETUTCDATE()
    );
    
    -- Try to insert duplicate (should fail)
    INSERT INTO dbo.Orders (Id, UserId, CartId, OrderNumber, TotalAmount, Currency, Status, CartSnapshot, CreatedAt, UpdatedAt)
    VALUES (
        NEWID(),
        'test-user-constraint',
        (SELECT CartId FROM dbo.Orders WHERE OrderNumber = 'TEST-CONSTRAINT-001'),
        'TEST-CONSTRAINT-002',
        100000,
        'IRR',
        0, -- Draft
        '[]',
        GETUTCDATE(),
        GETUTCDATE()
    );
    
    SELECT 'Constraint Test' AS CheckType, 'FAILED - Duplicate allowed' AS Result;
    
    -- Cleanup
    DELETE FROM dbo.Orders WHERE OrderNumber LIKE 'TEST-CONSTRAINT-%';
    
END TRY
BEGIN CATCH
    SELECT 'Constraint Test' AS CheckType, 'PASSED - Duplicate prevented' AS Result;
    
    -- Cleanup
    DELETE FROM dbo.Orders WHERE OrderNumber LIKE 'TEST-CONSTRAINT-%';
END CATCH
*/

PRINT 'CartId verification completed. Check results above.';