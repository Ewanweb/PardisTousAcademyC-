-- Emergency fix for duplicate CartId issue
-- Run this script directly on the database before applying migrations

-- 1. First, let's see what we're dealing with
SELECT 
    'Current state analysis' as Step,
    COUNT(*) as TotalOrders,
    SUM(CASE WHEN CartId IS NULL THEN 1 ELSE 0 END) as NullCartIds,
    SUM(CASE WHEN CartId = '00000000-0000-0000-0000-000000000000' THEN 1 ELSE 0 END) as DefaultGuidCartIds
FROM Orders;

-- 2. Show duplicate combinations that would cause the unique index to fail
SELECT 
    'Problematic duplicates' as Step,
    UserId,
    CartId,
    COUNT(*) as DuplicateCount,
    STRING_AGG(CAST(Id as NVARCHAR(36)), ', ') as OrderIds
FROM Orders 
WHERE Status IN (0, 1) -- Draft=0, PendingPayment=1 (only these are affected by the unique index)
GROUP BY UserId, CartId
HAVING COUNT(*) > 1;

-- 3. Fix NULL CartIds first
UPDATE Orders 
SET CartId = NEWID() 
WHERE CartId IS NULL;

-- 4. Fix duplicate CartIds by assigning new GUIDs to all but the oldest order
WITH DuplicateOrders AS (
    SELECT 
        Id,
        UserId,
        CartId,
        CreatedAt,
        ROW_NUMBER() OVER (PARTITION BY UserId, CartId ORDER BY CreatedAt ASC) as RowNum
    FROM Orders 
    WHERE Status IN (0, 1) -- Only active orders that will be affected by the unique index
)
UPDATE Orders 
SET CartId = NEWID()
FROM Orders o
INNER JOIN DuplicateOrders d ON o.Id = d.Id
WHERE d.RowNum > 1; -- Keep the first (oldest) order, update the rest

-- 5. Verify the fix
SELECT 
    'After fix verification' as Step,
    COUNT(*) as TotalActiveOrders,
    COUNT(DISTINCT CONCAT(UserId, '|', CAST(CartId as NVARCHAR(36)))) as UniqueUserCartCombinations
FROM Orders 
WHERE Status IN (0, 1);

-- 6. Final check - should return 0 rows if successful
SELECT 
    'Final duplicate check - should be empty' as Step,
    UserId,
    CartId,
    COUNT(*) as Count
FROM Orders 
WHERE Status IN (0, 1)
GROUP BY UserId, CartId
HAVING COUNT(*) > 1;

PRINT 'Data cleanup completed. You can now run the migrations.';