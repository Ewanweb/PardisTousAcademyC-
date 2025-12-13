# 📚 سیستم زمان‌بندی دوره‌ها - Course Schedule System

## 📋 خلاصه تغییرات

این سیستم امکان تعریف زمان‌های مختلف برگذاری برای هر دوره و ثبت‌نام دانشجویان در زمان‌بندی‌های مختلف را فراهم می‌کند.

### 🎯 هدف

- **مثال:** دوره "مقدماتی پایتون" می‌تواند در زمان‌های مختلف برگزار شود:
  - شنبه‌ها ساعت 12:00-14:00 (گروه صبح)
  - چهارشنبه‌ها ساعت 18:00-20:00 (گروه عصر)
- دانشجو می‌تواند یکی از این زمان‌ها را انتخاب کند
- مدرس می‌تواند لیست دانشجویان هر زمان‌بندی را ببیند

---

## 🆕 موجودیت‌های جدید (New Entities)

### 1. CourseSchedule (زمان‌بندی دوره)

**مسیر:** `Pardis.Domain/Courses/CourseSchedule.cs`

```csharp
public class CourseSchedule : BaseEntity
{
    public required Guid CourseId { get; set; }
    public required Course Course { get; set; }
    public required string Title { get; set; }           // "گروه صبح"
    public required int DayOfWeek { get; set; }          // 0=یکشنبه, 6=شنبه
    public required TimeOnly StartTime { get; set; }     // 12:00
    public required TimeOnly EndTime { get; set; }       // 14:00
    public required int MaxCapacity { get; set; }        // حداکثر 20 نفر
    public int EnrolledCount { get; set; } = 0;          // تعداد ثبت‌نام شده
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }             // "کلاس حضوری"

    // Navigation Properties
    public ICollection<UserCourseSchedule> StudentEnrollments { get; set; } = [];

    // Helper Methods
    public string GetDayName()                           // "شنبه"
    public string GetFullScheduleText()                  // "شنبه 12:00-14:00"
    public bool HasCapacity                              // آیا ظرفیت دارد؟
    public int RemainingCapacity                         // ظرفیت باقی‌مانده
}
```

### 2. UserCourseSchedule (ثبت‌نام در زمان‌بندی)

**مسیر:** `Pardis.Domain/Courses/UserCourseSchedule.cs`

```csharp
public class UserCourseSchedule
{
    public required string UserId { get; set; }
    public required User User { get; set; }
    public required Guid CourseScheduleId { get; set; }
    public required CourseSchedule CourseSchedule { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public StudentScheduleStatus Status { get; set; } = StudentScheduleStatus.Active;
    public int AttendedSessions { get; set; } = 0;      // تعداد حضور
    public int AbsentSessions { get; set; } = 0;        // تعداد غیبت
    public string? InstructorNotes { get; set; }        // یادداشت مدرس
}
```

### 3. CourseType (نوع دوره)

**مسیر:** `Pardis.Domain/Courses/CourseType.cs`

```csharp
public enum CourseType
{
    Online = 1,      // آنلاین
    InPerson = 2,    // حضوری
    Hybrid = 3       // ترکیبی
}
```

### 4. StudentScheduleStatus (وضعیت دانشجو)

**مسیر:** `Pardis.Domain/Courses/UserCourseSchedule.cs`

```csharp
public enum StudentScheduleStatus
{
    Active = 1,      // فعال
    Transferred = 2, // انتقال یافته
    Withdrawn = 3,   // انصراف
    Expelled = 4     // اخراج
}
```

---

## 🔄 تغییرات در موجودیت‌های موجود

### Course Entity

**مسیر:** `Pardis.Domain/Courses/Course.cs`

**اضافه شده:**

```csharp
public CourseType Type { get; set; }                           // نوع دوره
public string Location { get; set; }                           // محل برگزاری
public ICollection<CourseSchedule> Schedules { get; set; } = []; // زمان‌بندی‌ها
```

### AppDbContext

**مسیر:** `Pardis.Infrastructure/AppDbContext.cs`

**اضافه شده:**

```csharp
public DbSet<CourseSchedule> CourseSchedules { get; set; }
public DbSet<UserCourseSchedule> UserCourseSchedules { get; set; }

// تنظیمات روابط
builder.Entity<CourseSchedule>()
    .HasOne(cs => cs.Course)
    .WithMany(c => c.Schedules)
    .HasForeignKey(cs => cs.CourseId)
    .OnDelete(DeleteBehavior.Cascade);

builder.Entity<UserCourseSchedule>()
    .HasKey(ucs => new { ucs.UserId, ucs.CourseScheduleId });
```

---

## 📊 DTOs جدید

### 1. CourseScheduleDto

**مسیر:** `Pardis.Domain/Dto/Courses/CourseScheduleDto.cs`

```csharp
public class CourseScheduleDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required int DayOfWeek { get; set; }
    public required string DayName { get; set; }           // "شنبه"
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
    public required string TimeRange { get; set; }         // "12:00-14:00"
    public required string FullScheduleText { get; set; }  // "شنبه 12:00-14:00"
    public required int MaxCapacity { get; set; }
    public required int EnrolledCount { get; set; }
    public required int RemainingCapacity { get; set; }
    public required bool HasCapacity { get; set; }
    public required bool IsActive { get; set; }
    public string? Description { get; set; }
}
```

### 2. CreateCourseScheduleDto

```csharp
public class CreateCourseScheduleDto
{
    public required Guid CourseId { get; set; }
    public required string Title { get; set; }
    public required int DayOfWeek { get; set; }    // 0=یکشنبه, 6=شنبه
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
    public required int MaxCapacity { get; set; }
    public string? Description { get; set; }
}
```

### 3. ScheduleStudentDto

```csharp
public class ScheduleStudentDto
{
    public required string UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Mobile { get; set; }
    public required DateTime EnrolledAt { get; set; }
    public required string Status { get; set; }
    public required int AttendedSessions { get; set; }
    public required int AbsentSessions { get; set; }
    public string? InstructorNotes { get; set; }
}
```

### 4. تغییرات CourseResource

**مسیر:** `Pardis.Domain/Dto/Courses/CourseResource.cs`

**اضافه شده:**

```csharp
public required string Type { get; set; }                    // نوع دوره
public required string Location { get; set; }               // محل برگزاری
public List<CourseScheduleDto> Schedules { get; set; } = []; // زمان‌بندی‌ها
```

---

## 🔧 Commands و Handlers جدید

### 1. CreateScheduleCommand

**مسیر:** `Pardis.Application/Courses/Schedules/CreateScheduleCommand.cs`

```csharp
public record CreateScheduleCommand(CreateCourseScheduleDto Dto) : IRequest<OperationResult<CourseScheduleDto>>;
```

### 2. CreateScheduleHandler

**مسیر:** `Pardis.Application/Courses/Schedules/CreateScheduleHandler.cs`

**ویژگی‌ها:**

- بررسی وجود دوره
- اعتبارسنجی زمان (شروع < پایان)
- بررسی تداخل زمانی با زمان‌بندی‌های موجود
- بررسی ظرفیت (> 0)

### 3. EnrollInScheduleCommand

**مسیر:** `Pardis.Application/Courses/Schedules/EnrollInScheduleCommand.cs`

```csharp
public record EnrollInScheduleCommand(Guid CourseScheduleId, string UserId) : IRequest<OperationResult>;
```

### 4. EnrollInScheduleHandler

**مسیر:** `Pardis.Application/Courses/Schedules/EnrollInScheduleHandler.cs`

**بررسی‌ها:**

- وجود زمان‌بندی و فعال بودن آن
- ظرفیت باقی‌مانده
- ثبت‌نام قبلی کاربر در دوره اصلی
- عدم ثبت‌نام تکراری در همین زمان‌بندی
- عدم تداخل زمانی با دوره‌های دیگر کاربر

---

## 🔍 Queries جدید

### 1. GetScheduleStudentsQuery

**مسیر:** `Pardis.Query/Courses/Schedules/GetScheduleStudentsQuery.cs`

```csharp
public record GetScheduleStudentsQuery(Guid CourseScheduleId) : IRequest<List<ScheduleStudentDto>>;
```

### 2. GetScheduleStudentsHandler

**مسیر:** `Pardis.Query/Courses/Schedules/GetScheduleStudentsHandler.cs`

**خروجی:** لیست دانشجویان ثبت‌نام شده در یک زمان‌بندی خاص با اطلاعات حضور/غیبت

---

## 🌐 API Endpoints جدید

### CourseScheduleController

**مسیر:** `Endpoints/Api/Controllers/CourseScheduleController.cs`

**Base Route:** `/api/course/{courseId}/schedule`

#### 1. ایجاد زمان‌بندی جدید

```http
POST /api/course/{courseId}/schedule
Authorization: Bearer {token}
Roles: Admin, Manager, Instructor

Body:
{
  "title": "گروه صبح",
  "dayOfWeek": 6,
  "startTime": "12:00",
  "endTime": "14:00",
  "maxCapacity": 20,
  "description": "کلاس حضوری"
}

Response:
{
  "message": "زمان‌بندی با موفقیت ایجاد شد",
  "data": {
    "id": "guid",
    "title": "گروه صبح",
    "dayName": "شنبه",
    "timeRange": "12:00-14:00",
    "fullScheduleText": "شنبه 12:00-14:00",
    "maxCapacity": 20,
    "enrolledCount": 0,
    "remainingCapacity": 20,
    "hasCapacity": true
  }
}
```

#### 2. ثبت‌نام در زمان‌بندی

```http
POST /api/course/{courseId}/schedule/{scheduleId}/enroll
Authorization: Bearer {token}

Response:
{
  "message": "ثبت‌نام در زمان‌بندی با موفقیت انجام شد"
}
```

#### 3. دریافت لیست دانشجویان

```http
GET /api/course/{courseId}/schedule/{scheduleId}/students
Authorization: Bearer {token}
Roles: Admin, Manager, Instructor

Response:
{
  "data": [
    {
      "userId": "string",
      "fullName": "احمد محمدی",
      "email": "ahmad@example.com",
      "mobile": "09123456789",
      "enrolledAt": "2024-12-12T10:00:00Z",
      "status": "Active",
      "attendedSessions": 5,
      "absentSessions": 1,
      "instructorNotes": "دانشجوی فعال"
    }
  ]
}
```

---

## 🗄️ تغییرات دیتابیس (Migration)

### جداول جدید

#### 1. CourseSchedules

```sql
CREATE TABLE CourseSchedules (
    Id uniqueidentifier PRIMARY KEY,
    CourseId uniqueidentifier NOT NULL,
    Title nvarchar(max) NOT NULL,
    DayOfWeek int NOT NULL,
    StartTime time NOT NULL,
    EndTime time NOT NULL,
    MaxCapacity int NOT NULL,
    EnrolledCount int NOT NULL DEFAULT 0,
    IsActive bit NOT NULL DEFAULT 1,
    Description nvarchar(max) NULL,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NOT NULL,

    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
```

#### 2. UserCourseSchedules

```sql
CREATE TABLE UserCourseSchedules (
    UserId nvarchar(450) NOT NULL,
    CourseScheduleId uniqueidentifier NOT NULL,
    EnrolledAt datetime2 NOT NULL,
    Status int NOT NULL DEFAULT 1,
    AttendedSessions int NOT NULL DEFAULT 0,
    AbsentSessions int NOT NULL DEFAULT 0,
    InstructorNotes nvarchar(max) NULL,

    PRIMARY KEY (UserId, CourseScheduleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CourseScheduleId) REFERENCES CourseSchedules(Id) ON DELETE CASCADE
);
```

### ستون‌های جدید در جدول Courses

```sql
ALTER TABLE Courses ADD Type int NOT NULL DEFAULT 1;
ALTER TABLE Courses ADD Location nvarchar(max) NOT NULL DEFAULT '';
```

---

## 🔗 تغییرات Mapping

### MappingProfile

**مسیر:** `Pardis.Application/_Shared/MappingProfile.cs`

**اضافه شده:**

```csharp
// تبدیل CourseSchedule به CourseScheduleDto
CreateMap<CourseSchedule, CourseScheduleDto>()
    .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => src.GetDayName()))
    .ForMember(dest => dest.TimeRange, opt => opt.MapFrom(src => $"{src.StartTime:HH:mm}-{src.EndTime:HH:mm}"))
    .ForMember(dest => dest.FullScheduleText, opt => opt.MapFrom(src => src.GetFullScheduleText()))
    .ForMember(dest => dest.RemainingCapacity, opt => opt.MapFrom(src => src.RemainingCapacity))
    .ForMember(dest => dest.HasCapacity, opt => opt.MapFrom(src => src.HasCapacity));

// تبدیل Course به CourseResource
CreateMap<Course, CourseResource>()
    .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules))
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
```

---

## 📱 نحوه استفاده در Frontend

### 1. نمایش زمان‌بندی‌های دوره

```javascript
// دریافت اطلاعات دوره شامل زمان‌بندی‌ها
const course = await fetch(`/api/course/${courseId}`);
const schedules = course.data.schedules;

// نمایش زمان‌بندی‌ها
schedules.forEach((schedule) => {
  console.log(`${schedule.title}: ${schedule.fullScheduleText}`);
  console.log(`ظرفیت: ${schedule.enrolledCount}/${schedule.maxCapacity}`);
  console.log(`ظرفیت باقی‌مانده: ${schedule.remainingCapacity}`);
});
```

### 2. ثبت‌نام در زمان‌بندی

```javascript
// انتخاب زمان‌بندی توسط کاربر
const selectedScheduleId = "guid";

// ثبت‌نام
const response = await fetch(
  `/api/course/${courseId}/schedule/${selectedScheduleId}/enroll`,
  {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
  }
);

if (response.ok) {
  alert("ثبت‌نام با موفقیت انجام شد");
}
```

### 3. مدیریت زمان‌بندی‌ها (مدرس/ادمین)

```javascript
// ایجاد زمان‌بندی جدید
const newSchedule = {
  title: "گروه صبح",
  dayOfWeek: 6, // شنبه
  startTime: "12:00",
  endTime: "14:00",
  maxCapacity: 20,
  description: "کلاس حضوری",
};

const response = await fetch(`/api/course/${courseId}/schedule`, {
  method: "POST",
  headers: {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  },
  body: JSON.stringify(newSchedule),
});

// دریافت لیست دانشجویان
const students = await fetch(
  `/api/course/${courseId}/schedule/${scheduleId}/students`,
  {
    headers: { Authorization: `Bearer ${token}` },
  }
);
```

---

## ⚠️ نکات مهم برای Frontend

### 1. روزهای هفته

```javascript
const dayNames = {
  0: "یکشنبه",
  1: "دوشنبه",
  2: "سه‌شنبه",
  3: "چهارشنبه",
  4: "پنج‌شنبه",
  5: "جمعه",
  6: "شنبه",
};
```

### 2. وضعیت‌های دانشجو

```javascript
const statusNames = {
  Active: "فعال",
  Transferred: "انتقال یافته",
  Withdrawn: "انصراف",
  Expelled: "اخراج",
};
```

### 3. نوع دوره

```javascript
const courseTypes = {
  Online: "آنلاین",
  InPerson: "حضوری",
  Hybrid: "ترکیبی",
};
```

---

## 🔐 سطوح دسترسی

### ایجاد زمان‌بندی

- **Admin:** تمام دوره‌ها
- **Manager:** تمام دوره‌ها
- **Instructor:** فقط دوره‌های خودش

### ثبت‌نام در زمان‌بندی

- **همه کاربران لاگین شده** (پس از ثبت‌نام در دوره اصلی)

### مشاهده لیست دانشجویان

- **Admin:** تمام زمان‌بندی‌ها
- **Manager:** تمام زمان‌بندی‌ها
- **Instructor:** فقط زمان‌بندی‌های دوره‌های خودش

---

## 🚀 مراحل پیاده‌سازی

### Backend (تکمیل شده ✅)

1. ✅ ایجاد Entities جدید
2. ✅ تغییر Course Entity
3. ✅ ایجاد DTOs
4. ✅ ایجاد Commands/Handlers
5. ✅ ایجاد Queries
6. ✅ ایجاد Controller
7. ✅ تنظیم Mapping
8. ⏳ اجرای Migration

### Frontend (باید پیاده‌سازی شود)

1. 🔄 صفحه مدیریت زمان‌بندی‌ها
2. 🔄 کامپوننت انتخاب زمان‌بندی در ثبت‌نام
3. 🔄 صفحه لیست دانشجویان هر زمان‌بندی
4. 🔄 نمایش زمان‌بندی‌ها در جزئیات دوره
5. 🔄 فرم ایجاد زمان‌بندی جدید

---

## 📋 چک‌لیست تکمیل

### Backend

- [x] CourseSchedule Entity
- [x] UserCourseSchedule Entity
- [x] CourseType Enum
- [x] StudentScheduleStatus Enum
- [x] Course Entity Updates
- [x] AppDbContext Updates
- [x] DTOs (CourseScheduleDto, CreateCourseScheduleDto, ScheduleStudentDto)
- [x] CourseResource Updates
- [x] CreateScheduleCommand/Handler
- [x] EnrollInScheduleCommand/Handler
- [x] GetScheduleStudentsQuery/Handler
- [x] CourseScheduleController
- [x] MappingProfile Updates
- [ ] Migration اجرا شده
- [ ] Test API Endpoints

### Frontend (نیاز به پیاده‌سازی)

- [ ] Schedule Management Page
- [ ] Schedule Selection Component
- [ ] Students List Page
- [ ] Course Detail Updates
- [ ] Create Schedule Form
- [ ] Enroll in Schedule Flow

---

**📝 نکته:** این مستندات برای پیاده‌سازی Frontend کامل است و تمام جزئیات فنی و API endpoints لازم را شامل می‌شود.
