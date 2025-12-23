# راهنمای دیپلویمنت Plesk برای پروژه پردیس آکادمی

## مقدمه

این راهنما نحوه تنظیم CI/CD برای دیپلویمنت پروژه ASP.NET Core + React در هاست ویندوزی اشتراکی با Plesk را شرح می‌دهد.

## پیش‌نیازها

### 1. تنظیمات GitHub Secrets

در تنظیمات repository خود، Secrets زیر را اضافه کنید:

#### Staging Environment:

```
STAGING_FTP_HOST=ftp.staging.pardistous.com
STAGING_FTP_USER=staging_user
STAGING_FTP_PASSWORD=staging_password
STAGING_PLESK_AUTH=base64_encoded_user:password
STAGING_PLESK_API_URL=https://plesk.staging.com:8443
```

#### Production Environment:

```
PRODUCTION_FTP_HOST=ftp.pardistous.com
PRODUCTION_FTP_USER=production_user
PRODUCTION_FTP_PASSWORD=production_password
PRODUCTION_PLESK_AUTH=base64_encoded_user:password
PRODUCTION_PLESK_API_URL=https://plesk.pardistous.com:8443
SLACK_WEBHOOK=https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK
```

### 2. تنظیمات Plesk

#### فعال‌سازی ASP.NET Core

1. در Plesk Panel، به **Websites & Domains** بروید
2. روی دامنه خود کلیک کنید
3. **Hosting Settings** را انتخاب کنید
4. **ASP.NET Core support** را فعال کنید
5. **.NET version** را روی **8.0** تنظیم کنید

#### تنظیم Application Pool

1. در **IIS Settings** بروید
2. **Application Pools** را انتخاب کنید
3. Application Pool مربوط به دامنه را پیدا کنید
4. **.NET CLR Version** را روی **No Managed Code** تنظیم کنید
5. **Managed Pipeline Mode** را روی **Integrated** تنظیم کنید

#### فعال‌سازی Plesk API

1. در **Tools & Settings** بروید
2. **API Access** را انتخاب کنید
3. **XML API** و **REST API** را فعال کنید
4. IP آدرس GitHub Actions را به whitelist اضافه کنید

## ساختار دیپلویمنت

### 1. ساختار فایل‌ها در سرور

```
/httpdocs/
├── api/                    # ASP.NET Core API
│   ├── Api.dll
│   ├── web.config
│   ├── appsettings.json
│   └── ...
├── index.html             # React App
├── static/                # React Static Files
├── .htaccess             # URL Rewriting
└── web.config            # IIS Configuration
```

### 2. فرآیند CI/CD

#### مرحله 1: Build & Test

- Build ASP.NET Core برای Windows (win-x64)
- Build React App برای Production
- اجرای Unit Tests
- اجرای ESLint و Tests فرانت‌اند

#### مرحله 2: Package Creation

- ایجاد ساختار deployment
- کپی فایل‌های API و Frontend
- ایجاد web.config و .htaccess
- فشرده‌سازی در ZIP

#### مرحله 3: Deployment

- آپلود ZIP به سرور via FTP
- استخراج فایل‌ها via Plesk API
- اجرای Database Migrations
- راه‌اندازی مجدد Application Pool
- Health Check

## تنظیمات Database

### Connection String در appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=pardis_academy;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;"
  }
}
```

### برای SQL Server در Plesk:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=pardis_academy;User Id=db_user;Password=db_password;TrustServerCertificate=true;"
  }
}
```

## تنظیمات Environment Variables

### در Plesk Panel:

1. **Websites & Domains** > **Hosting Settings**
2. در بخش **Environment Variables** موارد زیر را اضافه کنید:

```
ASPNETCORE_ENVIRONMENT=Production
JWT_SECRET=your-jwt-secret-key
SMTP_HOST=smtp.your-provider.com
SMTP_PORT=587
SMTP_USER=your-email@domain.com
SMTP_PASSWORD=your-email-password
```

## فایل‌های تنظیمات

### web.config (Root)

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="API Routes" stopProcessing="true">
          <match url="^api/.*" />
          <action type="Rewrite" url="api/{R:0}" />
        </rule>
        <rule name="React Router" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/api/" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

### .htaccess (برای Apache)

```apache
Options -MultiViews
RewriteEngine On
RewriteCond %{REQUEST_FILENAME} !-f
RewriteRule ^ index.html [QSA,L]
```

## اجرای دستی دیپلویمنت

### 1. Build Local

```powershell
# Build Backend
dotnet publish Endpoints/Api/Api.csproj -c Release -o ./publish/api --runtime win-x64 --self-contained false

# Build Frontend
cd react-frontend-code
npm run build
cd ..
```

### 2. آپلود via FTP

```powershell
# استفاده از PowerShell Script
.\deployment-scripts\ftp-deploy.ps1 -FtpHost "ftp.pardistous.com" -FtpUser "user" -FtpPassword "pass" -LocalPath "./publish" -RemotePath "/httpdocs"
```

### 3. Database Migration

```bash
# در Plesk File Manager یا SSH
cd /httpdocs/api
dotnet ef database update
```

## مانیتورینگ و عیب‌یابی

### 1. لاگ‌ها

- **IIS Logs**: در Plesk Panel > **Logs**
- **Application Logs**: در `/httpdocs/api/logs/`
- **Event Viewer**: در Windows Server

### 2. Health Check Endpoints

```
GET /api/health
GET /api/health/ready
GET /api/health/live
```

### 3. مشکلات رایج

#### خطای 500.30

- بررسی نصب ASP.NET Core Runtime
- بررسی web.config
- بررسی Application Pool settings

#### خطای Database Connection

- بررسی Connection String
- بررسی دسترسی‌های Database User
- بررسی Firewall

#### مشکل React Routing

- بررسی URL Rewrite rules
- بررسی .htaccess یا web.config

## بهینه‌سازی Performance

### 1. IIS Settings

```xml
<system.webServer>
  <urlCompression doStaticCompression="true" doDynamicCompression="true" />
  <httpCompression>
    <dynamicTypes>
      <add mimeType="application/json" enabled="true" />
    </dynamicTypes>
  </httpCompression>
</system.webServer>
```

### 2. Caching

```xml
<system.webServer>
  <staticContent>
    <clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="30.00:00:00" />
  </staticContent>
</system.webServer>
```

### 3. Compression

- فعال‌سازی Gzip در IIS
- استفاده از CDN برای Static Files
- بهینه‌سازی Images

## امنیت

### 1. Security Headers

```xml
<httpProtocol>
  <customHeaders>
    <add name="X-Content-Type-Options" value="nosniff" />
    <add name="X-Frame-Options" value="DENY" />
    <add name="X-XSS-Protection" value="1; mode=block" />
  </customHeaders>
</httpProtocol>
```

### 2. Request Filtering

```xml
<security>
  <requestFiltering>
    <requestLimits maxAllowedContentLength="104857600" />
    <fileExtensions>
      <add fileExtension=".config" allowed="false" />
    </fileExtensions>
  </requestFiltering>
</security>
```

## Backup و Recovery

### 1. Automated Backup

- تنظیم Scheduled Backup در Plesk
- Backup قبل از هر deployment
- ذخیره Database Backup

### 2. Rollback Strategy

```powershell
# Restore from backup
.\deployment-scripts\rollback.ps1 -BackupName "pre-deployment-20231223-143000"
```

## نکات مهم

1. **همیشه Backup بگیرید** قبل از deployment
2. **Test کنید** در staging environment
3. **Monitor کنید** logs بعد از deployment
4. **Database Migration** را با احتیاط انجام دهید
5. **SSL Certificate** را تنظیم کنید
6. **Domain Binding** را بررسی کنید

این راهنما تمام جنبه‌های دیپلویمنت در Plesk را پوشش می‌دهد و CI/CD pipeline کاملی برای پروژه شما فراهم می‌کند.
