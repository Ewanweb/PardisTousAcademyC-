-- Script to identify and fix NULL GUID issues in PaymentAttempts table
-- Run this script to diagnose and fix SqlNullValueException issues

-- 1. Check for NULL values in GUID columns of PaymentAttempts table
SELECT 
    'PaymentAttempts' as TableName,
    COUNT(*) as TotalRecords,
    SUM(CASE WHEN Id IS NULL THEN 1 ELSE 0 END) as NullIds,
    SUM(CASE WHEN OrderId IS NULL THEN 1 ELSE 0 END) as NullOrderIds
FROM PaymentAttempts;

-- 2. Check for NULL values in GUID columns of Orders table
SELECT 
    'Orders' as TableName,
    COUNT(*) as TotalRecords,
    SUM(CASE WHEN Id IS NULL THEN 1 ELSE 0 END) as NullIds,
    SUM(CASE WHEN CartId IS NULL THEN 1 ELSE 0 END) as NullCartIds
FROM Orders;

-- 3. Find PaymentAttempts with NULL OrderId (this is likely the cause)
SELECT 
    Id,
    OrderId,
    UserId,
    Method,
    Amount,
    Status,
    CreatedAt
FROM PaymentAttempts 
WHERE OrderId IS NULL;

-- 4. Find Orders with NULL CartId
SELECT 
    Id,
    UserId,
    OrderNumber,
    CartId,
    Status,
    CreatedAt
FROM Orders 
WHERE CartId IS NULL;

-- 5. Check for orphaned PaymentAttempts (PaymentAttempts without valid Orders)
SELECT 
    pa.Id as PaymentAttemptId,
    pa.OrderId,
    pa.UserId,
    pa.Method,
    pa.Amount,
    pa.Status,
    pa.CreatedAt
FROM PaymentAttempts pa
LEFT JOIN Orders o ON pa.OrderId = o.Id
WHERE o.Id IS NULL;

-- 6. CLEANUP SCRIPT (UNCOMMENT AND RUN CAREFULLY AFTER BACKUP)
-- WARNING: This will delete orphaned PaymentAttempts. Make sure to backup first!

/*
-- Delete PaymentAttempts with NULL OrderId
DELETE FROM PaymentAttempts WHERE OrderId IS NULL;

-- Delete orphaned PaymentAttempts (those pointing to non-existent Orders)
DELETE pa FROM PaymentAttempts pa
LEFT JOIN Orders o ON pa.OrderId = o.Id
WHERE o.Id IS NULL;

-- Update Orders with NULL CartId to use a default GUID (if needed)
-- UPDATE Orders SET CartId = NEWID() WHERE CartId IS NULL;
*/

-- 7. Verify data integrity after cleanup
SELECT 
    'After Cleanup - PaymentAttempts' as TableName,
    COUNT(*) as TotalRecords,
    SUM(CASE WHEN Id IS NULL THEN 1 ELSE 0 END) as NullIds,
    SUM(CASE WHEN OrderId IS NULL THEN 1 ELSE 0 END) as NullOrderIds
FROM PaymentAttempts;

SELECT 
    'After Cleanup - Orders' as TableName,
    COUNT(*) as TotalRecords,
    SUM(CASE WHEN Id IS NULL THEN 1 ELSE 0 END) as NullIds,
    SUM(CASE WHEN CartId IS NULL THEN 1 ELSE 0 END) as NullCartIds
FROM Orders;