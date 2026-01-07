# SqlNullValueException Fix Documentation

## مشکل

خطای `System.Data.SqlTypes.SqlNullValueException` هنگام خواندن سفارش‌ها و تلاش‌های پرداخت رخ می‌دهد. این خطا زمانی اتفاق می‌افتد که Entity Framework سعی می‌کند مقادیر NULL را به عنوان GUID غیرقابل null بخواند.

## علت احتمالی

- وجود مقادیر NULL در ستون‌های GUID در جدول PaymentAttempts
- PaymentAttempts یتیم (بدون Order مرتبط)
- مشکلات در کانفیگوریشن Entity Framework

## راه‌حل پیاده‌سازی شده

### 1. OrderExtensions کلاس

کلاس `OrderExtensions` متدهای ایمن برای کار با PaymentAttempts فراهم می‌کند:

- `SafeLoadPaymentAttemptsAsync`: بارگذاری ایمن PaymentAttempts
- `SafeGetOrdersWithPaymentAttemptsAsync`: دریافت ایمن لیست سفارش‌ها
- `GetSafePaymentAttempts`: دریافت ایمن PaymentAttempts از Order

### 2. بروزرسانی OrderRepository

تمام متدهای OrderRepository برای استفاده از متدهای ایمن بروزرسانی شدند:

- `GetByUserIdAsync`
- `GetByIdAsync`
- `GetByOrderNumberAsync`
- `GetPendingOrdersAsync`

### 3. بروزرسانی GetMyOrdersHandler

Handler برای مدیریت بهتر خطاها و استفاده از متدهای ایمن بروزرسانی شد.

### 4. اسکریپت تشخیص و رفع مشکل

فایل `FixNullGuidIssues.sql` شامل کوئری‌هایی برای:

- تشخیص مقادیر NULL در جداول
- یافتن PaymentAttempts یتیم
- پاک‌سازی داده‌های نامعتبر (با احتیاط)

## نحوه استفاده

### 1. اجرای اسکریپت تشخیص

```sql
-- اجرای بخش‌های تشخیصی اسکریپت FixNullGuidIssues.sql
```

### 2. پشتیبان‌گیری از دیتابیس

قبل از اجرای بخش پاک‌سازی، حتماً از دیتابیس پشتیبان بگیرید.

### 3. اجرای پاک‌سازی (اختیاری)

در صورت یافتن داده‌های نامعتبر، بخش پاک‌سازی اسکریپت را اجرا کنید.

## مزایای راه‌حل

1. **مقاوم در برابر خطا**: برنامه دیگر با SqlNullValueException متوقف نمی‌شود
2. **حفظ عملکرد**: در صورت عدم وجود مشکل، عملکرد تغییر نمی‌کند
3. **قابل نگهداری**: کد تمیز و قابل فهم
4. **انعطاف‌پذیر**: می‌تواند با انواع مختلف مشکلات NULL GUID کنار بیاید

## نکات مهم

- این راه‌حل مشکل را در سطح کد حل می‌کند، اما بهتر است علت اصلی در دیتابیس نیز رفع شود
- همیشه قبل از اجرای اسکریپت‌های پاک‌سازی، پشتیبان بگیرید
- در صورت تکرار مشکل، بررسی کنید که آیا کد ایجاد PaymentAttempt مقادیر صحیح تنظیم می‌کند یا خیر
