# Enhanced Slider API Documentation

## Overview

این API برای مدیریت اسلایدهای اصلی (Hero Slides) و استوری‌های موفقیت (Success Stories) با قابلیت‌های پیشرفته طراحی شده است.

## New Features Added

### Hero Slides Enhancements

- **Badge**: نشان یا برچسب برای اسلاید
- **Primary Action**: اکشن اصلی با برچسب و لینک
- **Secondary Action**: اکشن ثانویه با برچسب و لینک
- **Stats**: آمار و اطلاعات اضافی به صورت JSON
- **Backward Compatibility**: پشتیبانی از فیلدهای قدیمی `LinkUrl` و `ButtonText`

### Success Stories Enhancements

- **Subtitle**: زیرعنوان برای استوری
- **Badge**: نشان یا برچسب
- **Type**: نوع استوری (`success`, `video`)
- **Action**: اکشن با برچسب و لینک
- **Stats**: آمار و اطلاعات اضافی
- **Duration**: مدت زمان برای ویدیوها (میلی‌ثانیه)
- **Type Filtering**: فیلتر بر اساس نوع استوری

## API Endpoints

### Hero Slides

#### GET /api/heroslides

دریافت لیست اسلایدهای اصلی

**Parameters:**

- `includeInactive` (bool): شامل اسلایدهای غیرفعال
- `includeExpired` (bool): شامل اسلایدهای منقضی شده
- `adminView` (bool): نمای مدیریتی

#### GET /api/heroslides/active

دریافت اسلایدهای فعال برای نمایش عمومی

#### GET /api/heroslides/{id}

دریافت اسلاید با شناسه مشخص

#### POST /api/heroslides

ایجاد اسلاید جدید (نیاز به احراز هویت)

#### PUT /api/heroslides/{id}

به‌روزرسانی اسلاید (نیاز به احراز هویت)

#### DELETE /api/heroslides/{id}

حذف اسلاید (نیاز به احراز هویت)

### Success Stories

#### GET /api/successstories

دریافت لیست استوری‌های موفقیت

**Parameters:**

- `includeInactive` (bool): شامل استوری‌های غیرفعال
- `includeExpired` (bool): شامل استوری‌های منقضی شده
- `adminView` (bool): نمای مدیریتی
- `type` (string): فیلتر بر اساس نوع (`success`, `video`)

#### GET /api/successstories/active

دریافت استوری‌های فعال برای نمایش عمومی

#### GET /api/successstories/type/{type}

دریافت استوری‌ها بر اساس نوع مشخص

#### GET /api/successstories/{id}

دریافت استوری با شناسه مشخص

#### POST /api/successstories

ایجاد استوری جدید (نیاز به احراز هویت)

#### PUT /api/successstories/{id}

به‌روزرسانی استوری (نیاز به احراز هویت)

#### DELETE /api/successstories/{id}

حذف استوری (نیاز به احراز هویت)

## Data Models

### HeroSlideResource

```json
{
  "id": "guid",
  "title": "string",
  "description": "string",
  "imageUrl": "string",
  "badge": "string",
  "primaryAction": {
    "label": "string",
    "link": "string"
  },
  "secondaryAction": {
    "label": "string",
    "link": "string"
  },
  "stats": [
    {
      "icon": "string",
      "value": "string",
      "label": "string"
    }
  ],
  "order": "number",
  "isActive": "boolean",
  "isPermanent": "boolean",
  "expiresAt": "datetime",
  "timeRemaining": "timespan",
  "isExpired": "boolean",
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "createdByUserId": "guid",
  // Legacy fields for backward compatibility
  "linkUrl": "string",
  "buttonText": "string"
}
```

### SuccessStoryResource

```json
{
  "id": "guid",
  "title": "string",
  "subtitle": "string",
  "description": "string",
  "imageUrl": "string",
  "badge": "string",
  "type": "string", // "success" or "video"
  "studentName": "string",
  "courseName": "string",
  "action": {
    "label": "string",
    "link": "string"
  },
  "stats": [
    {
      "value": "string",
      "label": "string"
    }
  ],
  "duration": "number", // milliseconds for video type
  "courseId": "guid",
  "order": "number",
  "isActive": "boolean",
  "isPermanent": "boolean",
  "expiresAt": "datetime",
  "timeRemaining": "timespan",
  "isExpired": "boolean",
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "createdByUserId": "guid",
  // Legacy field for backward compatibility
  "linkUrl": "string"
}
```

### CreateHeroSlideDto

```json
{
  "title": "string", // required
  "description": "string",
  "imageFile": "file", // upload file
  "imageUrl": "string", // or provide URL
  "badge": "string",
  "primaryActionLabel": "string",
  "primaryActionLink": "string",
  "secondaryActionLabel": "string",
  "secondaryActionLink": "string",
  "stats": [
    {
      "icon": "string",
      "value": "string",
      "label": "string"
    }
  ],
  "order": "number",
  "isPermanent": "boolean",
  "expiresAt": "datetime",
  // Legacy fields for backward compatibility
  "linkUrl": "string",
  "buttonText": "string"
}
```

### CreateSuccessStoryDto

```json
{
  "title": "string", // required
  "subtitle": "string",
  "description": "string",
  "imageFile": "file", // upload file
  "imageUrl": "string", // or provide URL
  "badge": "string",
  "type": "string", // "success" or "video", default: "success"
  "studentName": "string",
  "courseName": "string",
  "actionLabel": "string",
  "actionLink": "string",
  "stats": [
    {
      "value": "string",
      "label": "string"
    }
  ],
  "duration": "number", // milliseconds for video type
  "courseId": "guid",
  "order": "number",
  "isPermanent": "boolean",
  "expiresAt": "datetime",
  // Legacy field for backward compatibility
  "linkUrl": "string"
}
```

## Database Changes

### New Columns Added to HeroSlides:

- `Badge` (NVARCHAR(100))
- `PrimaryActionLabel` (NVARCHAR(100))
- `PrimaryActionLink` (NVARCHAR(500))
- `SecondaryActionLabel` (NVARCHAR(100))
- `SecondaryActionLink` (NVARCHAR(500))
- `StatsJson` (NVARCHAR(MAX))

### New Columns Added to SuccessStories:

- `Subtitle` (NVARCHAR(100))
- `Badge` (NVARCHAR(100))
- `Type` (NVARCHAR(50), DEFAULT 'success')
- `ActionLabel` (NVARCHAR(100))
- `ActionLink` (NVARCHAR(500))
- `StatsJson` (NVARCHAR(MAX))
- `Duration` (INT)

### Indexes Added:

- Performance indexes on active/permanent/order combinations
- Indexes on expiration dates
- Index on success story type

## Migration Notes

1. **Backward Compatibility**: تمام فیلدهای قدیمی حفظ شده‌اند
2. **Data Migration**: داده‌های موجود به فیلدهای جدید منتقل می‌شوند
3. **Legacy Support**: API همچنان از فیلدهای قدیمی پشتیبانی می‌کند
4. **AutoMapper**: از AutoMapper برای تبدیل داده‌ها استفاده می‌شود

## Testing

برای تست API می‌توانید از endpoints زیر استفاده کنید:

```bash
# دریافت اسلایدهای فعال
GET /api/heroslides/active

# دریافت استوری‌های موفقیت فعال
GET /api/successstories/active

# دریافت استوری‌های ویدیویی
GET /api/successstories/type/video

# دریافت استوری‌های موفقیت
GET /api/successstories/type/success
```

## Frontend Integration

فرانت‌اند می‌تواند از فیلدهای جدید برای نمایش بهتر استفاده کند:

1. **Stats**: نمایش آمار و اطلاعات اضافی
2. **Actions**: دکمه‌های اکشن اصلی و ثانویه
3. **Badge**: نمایش نشان‌ها و برچسب‌ها
4. **Type Filtering**: فیلتر استوری‌ها بر اساس نوع
5. **Duration**: نمایش مدت زمان ویدیوها
