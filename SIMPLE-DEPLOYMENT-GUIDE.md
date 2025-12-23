# راهنمای دیپلویمنت ساده برای هاست اشتراکی

## مقدمه

این راهنما برای هاست‌های اشتراکی که فقط دسترسی FTP دارند و Plesk API ندارند طراحی شده است.

## GitHub Secrets مورد نیاز

فقط این Secrets را در GitHub تنظیم کنید:

### Production:

```
PRODUCTION_FTP_HOST=ftp.pardistous.com
PRODUCTION_FTP_USER=your_ftp_username
PRODUCTION_FTP_PASSWORD=your_ftp_password
PRODUCTION_DATABASE_CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=PardisAcademy;User Id=db_user;Password=db_pass;TrustServerCertificate=True;
PRODUCTION_JWT_SECRET_KEY=YourSuperSecretJWTKey2024!@#$%^&*()
PRODUCTION_SMTP_HOST=smtp.gmail.com
PRODUCTION_SMTP_PORT=587
PRODUCTION_SMTP_USER=your-email@gmail.com
PRODUCTION_SMTP_PASSWORD=your-app-password
PRODUCTION_FROM_EMAIL=noreply@pardistous.com
```

### Staging:

```
STAGING_FTP_HOST=ftp.staging.pardistous.com
STAGING_FTP_USER=staging_ftp_username
STAGING_FTP_PASSWORD=staging_ftp_password
STAGING_DATABASE_CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=PardisAcademy_Staging;User Id=staging_user;Password=staging_pass;TrustServerCelerificate=True;
STAGING_JWT_SECRET_KEY=StagingJWTKey2024!@#$%^&*()
STAGING_SMTP_HOST=smtp.gmail.com
STAGING_SMTP_PORT=587
STAGING_SMTP_USER=staging@gmail.com
STAGING_SMTP_PASSWORD=staging-app-password
STAGING_FROM_EMAIL=staging@pardistous.com
```

### اختیاری (برای اطلاع‌رسانی):

```
SLACK_WEBHOOK=https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK
```

## ساختار فایل‌ها در سرور

بعد از deployment، ساختار فایل‌ها در سرور به این شکل خواهد بود:

```
/httpdocs/ (یا public_html)
├── api/                    # ASP.NET Core API
│   ├── Api.dll
│   ├── appsettings.json
│   ├── web.config
│   └── ...
├── index.html             # React App
├── static/                # React Static Files
├── web.config            # URL Rewriting
├── .htaccess            # Apache fallback
└── deployment-info.json  # Deployment metadata
```

## نحوه کار CI/CD

### 1. Trigger

- Push به `main` branch → Production deployment
- Push به `develop` branch → Staging deployment

### 2. Build Process

- Build ASP.NET Core برای Windows
- Build React App
- Run Tests (اختیاری)

### 3. Package Creation

- ایجاد ساختار deployment
- تنظیم appsettings.json با مقادیر واقعی
- ایجاد web.config files
- ایجاد .htaccess برای Apache

### 4. Deployment

- آپلود فایل‌ها via FTP
- تست سلامت سایت و API

## تنظیمات دستی مورد نیاز

### 1. در Plesk Panel

#### Database Setup:

1. **Websites & Domains** > **Databases**
2. **Add Database**
3. نام: `PardisAcademy`
4. User و Password تنظیم کنید

#### ASP.NET Core Setup:

1. **Websites & Domains** > **Hosting Settings**
2. **ASP.NET Core support** را فعال کنید
3. **.NET version** را روی **8.0** تنظیم کنید

#### Application Pool:

1. **IIS Settings** > **Application Pools**
2. **.NET CLR Version**: **No Managed Code**
3. **Managed Pipeline Mode**: **Integrated**

### 2. تنظیم Subdirectory برای API

اگر می‌خواهید API در subdirectory جداگانه باشد:

1. در Plesk، **Subdirectories** بروید
2. یک subdirectory به نام `api` ایجاد کنید
3. **ASP.NET Core** را برای آن فعال کنید

## Database Migration

### روش 1: دستی (پیشنهادی)

```bash
# در File Manager یا FTP
cd /httpdocs/api
dotnet ef database update
```

### روش 2: از طریق Package Manager Console

```bash
# در Visual Studio
Update-Database -ConnectionString "your-connection-string"
```

### روش 3: SQL Script

```bash
# تولید SQL Script
dotnet ef migrations script --output migration.sql
# سپس SQL را در Plesk Database Manager اجرا کنید
```

## عیب‌یابی مشکلات رایج

### 1. خطای 500.30 - ASP.NET Core app failed to start

**راه‌حل:**

- بررسی کنید ASP.NET Core Runtime نصب باشد
- web.config را بررسی کنید
- logs را در `/httpdocs/api/logs/` چک کنید

### 2. React Routes کار نمی‌کند

**راه‌حل:**

- web.config در root directory باشد
- URL Rewrite module در IIS فعال باشد
- .htaccess برای Apache موجود باشد

### 3. API در دسترس نیست

**راه‌حل:**

- بررسی کنید `/api/` routes درست تنظیم شده باشند
- Connection String صحیح باشد
- Database accessible باشد

### 4. خطای CORS

**راه‌حل:**
در `Program.cs` اضافه کنید:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors();
```

## تست Deployment

### 1. Health Checks

```bash
# Frontend
curl https://pardistous.com/

# API Health
curl https://pardistous.com/api/health

# API Specific endpoint
curl https://pardistous.com/api/courses
```

### 2. Browser Tests

- بروید به سایت اصلی
- تست کنید React routing کار می‌کند
- تست کنید API calls موفق هستند
- Developer Tools را برای errors چک کنید

## مانیتورینگ

### 1. Deployment Info

فایل `deployment-info.json` اطلاعات deployment را نشان می‌دهد:

```json
{
  "DeploymentDate": "2024-01-15 14:30:00",
  "GitCommit": "abc123...",
  "GitBranch": "main",
  "Environment": "Production",
  "BuildNumber": "42"
}
```

### 2. Logs

- **IIS Logs**: در Plesk Panel > Logs
- **Application Logs**: در `/httpdocs/api/logs/`
- **Browser Console**: برای frontend errors

## بهینه‌سازی Performance

### 1. Static Files Caching

در web.config اضافه کنید:

```xml
<staticContent>
  <clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="30.00:00:00" />
</staticContent>
```

### 2. Compression

```xml
<urlCompression doStaticCompression="true" doDynamicCompression="true" />
```

### 3. CDN

برای فایل‌های static از CDN استفاده کنید.

## امنیت

### 1. HTTPS

- SSL Certificate را در Plesk فعال کنید
- HTTP به HTTPS redirect کنید

### 2. Security Headers

در web.config موجود است:

```xml
<customHeaders>
  <add name="X-Content-Type-Options" value="nosniff" />
  <add name="X-Frame-Options" value="DENY" />
  <add name="X-XSS-Protection" value="1; mode=block" />
</customHeaders>
```

## Rollback

اگر مشکلی پیش آمد:

### 1. FTP Rollback

- فایل‌های قبلی را backup داشته باشید
- از طریق FTP فایل‌های قدیمی را restore کنید

### 2. Database Rollback

- Database backup بگیرید قبل از migration
- در صورت مشکل، backup را restore کنید

## نکات مهم

1. **همیشه Test کنید** در staging قبل از production
2. **Backup بگیرید** قبل از هر deployment
3. **Monitor کنید** logs بعد از deployment
4. **Connection String** را secure نگه دارید
5. **SSL** را فعال کنید
6. **Database Migration** را با احتیاط انجام دهید

این راهنما برای هاست‌های اشتراکی ساده و بدون نیاز به API یا SSH طراحی شده است.
