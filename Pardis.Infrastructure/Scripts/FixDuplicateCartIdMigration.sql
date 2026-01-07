-- Script to fix duplicate CartId issue before running migration
-- This script should be run BEFORE applying the migration

-- 1. First, let's see the current state of Orders with NULL or duplicate CartIds
SELECT 
    'Current Orders with NULL CartId' as Description,
    COUNT(*) as Count
FROM Orders 
WHERE CartId IS NULL;

SELECT 
    'Current Orders with default GUID CartId' as Description,
    COUNT(*) as Count
FROM Orders 
WHERE CartId = '00000000-0000-0000-0000-000000000000';

-- 2. Find duplicate UserId + CartId combinations that would violate the unique constraint
SELECT 
    UserId,
    CartId,
    COUNT(*) as DuplicateCount,
    STRING_AGG(CAST(Id as NVARCHAR(36)), ', ') as OrderIds
FROM Orders 
WHERE Status IN (0, 1) -- Draft=0, PendingPayment=1
GROUP BY UserId, CartId
HAVING COUNT(*) > 1;

-- 3. SOLUTION: Update Orders to have unique CartIds
-- Generate new GUIDs for Orders that have NULL or duplicate CartIds

-- Step 3a: Update NULL CartIds with new GUIDs
UPDATE Orders 
SET CartId = NEWID() 
WHERE CartId IS NULL;

-- Step 3b: For Orders with duplicate CartIds (including default GUID), assign new unique GUIDs
-- We'll keep the oldest order with the original CartId and update the rest
WITH DuplicateOrders AS (
    SELECT 
        Id,
        UserId,
        CartId,
        ROW_NUMBER() OVER (PARTITION BY UserId, CartId ORDER BY CreatedAt ASC) as RowNum
    FROM Orders 
    WHERE Status IN (0, 1) -- Only for active orders that will be affected by the unique index
)
UPDATE Orders 
SET CartId = NEWID()
FROM Orders o
INNER JOIN DuplicateOrders d ON o.Id = d.Id
WHERE d.RowNum > 1; -- Keep the first (oldest) order, update the rest

-- 4. Verify the fix - check for remaining duplicates
SELECT 
    'After Fix - Remaining duplicates' as Description,
    UserId,
    CartId,
    COUNT(*) as Count
FROM Orders 
WHERE Status IN (0, 1)
GROUP BY UserId, CartId
HAVING COUNT(*) > 1;

-- 5. Final verification - should return 0 rows
SELECT 
    'Final Check - Orders ready for unique index' as Description,
    COUNT(*) as TotalActiveOrders,
    COUNT(DISTINCT CONCAT(UserId, '|', CartId)) as UniqueUserCartCombinations
FROM Orders 
WHERE Status IN (0, 1);

-- The counts above should be equal for the migration to succeed