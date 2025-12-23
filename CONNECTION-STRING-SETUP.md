# راهنمای تنظیم Connection String برای هاست Plesk

## مقدمه

این راهنما نحوه تنظیم Connection String برای پروژه در محیط‌های مختلف (Development، Staging، Production) را شرح می‌دهد.

## انواع Connection String

### 1. SQL Server (پیشنهادی برای Production)

```json
"Server=your-server.database.windows.net;Database=PardisAcademy;User Id=your-username;Password=your-password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### 2. SQL Server Express (برای هاست‌های اشتراکی)

```json
"Server=localhost\\SQLEXPRESS;Database=PardisAcademy;User Id=db_user;Password=db_password;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=True;"
```

### 3. MySQL (اگر هاست از MySQL استفاده می‌کند)

```json
"Server=localhost;Database=PardisAcademy;Uid=mysql_user;Pwd=mysql_password;SslMode=Required;"
```

### 4. SQLite (برای تست محلی)

```json
"Data Source=PardisAcademy.db"
```

## تنظیمات GitHub Secrets

در GitHub Repository خود، به **Settings > Secrets and variables > Actions** بروید و Secrets زیر را اضافه کنید:

### Production Secrets:

```
PRODUCTION_DATABASE_CONNECTION_STRING=Server=prod-server;Database=PardisAcademy;User Id=prod_user;Password=prod_pass;Encrypt=True;TrustServerCertificate=False;
PRODUCTION_JWT_SECRET_KEY=YourSuperSecretJWTKeyForProduction2024!@#$%^&*()
PRODUCTION_SMTP_HOST=smtp.gmail.com
PRODUCTION_SMTP_PORT=587
PRODUCTION_SMTP_USER=your-email@gmail.com
PRODUCTION_SMTP_PASSWORD=your-app-password
PRODUCTION_FROM_EMAIL=noreply@pardistous.com
```

### Staging Secrets:

```
STAGING_DATABASE_CONNECTION_STRING=Server=staging-server;Database=PardisAcademy_Staging;User Id=staging_user;Password=staging_pass;Encrypt=True;TrustServerCertificate=False;
STAGING_JWT_SECRET_KEY=YourStagingJWTKeyForTesting2024!@#$%^&*()
STAGING_SMTP_HOST=smtp.gmail.com
STAGING_SMTP_PORT=587
STAGING_SMTP_USER=staging-email@gmail.com
STAGING_SMTP_PASSWORD=staging-app-password
STAGING_FROM_EMAIL=staging@pardistous.com
```

## تنظیمات Plesk

### 1. ایجاد Database در Plesk

#### برای SQL Server:

1. در Plesk Panel به **Websites & Domains** بروید
2. **Databases** را انتخاب کنید
3. **Add Database** کلیک کنید
4. نام database: `PardisAcademy`
5. نوع: `Microsoft SQL Server`
6. User و Password تنظیم کنید

#### برای MySQL:

1. در Plesk Panel به **Websites & Domains** بروید
2. **Databases** را انتخاب کنید
3. **Add Database** کلیک کنید
4. نام database: `PardisAcademy`
5. نوع: `MySQL`
6. User و Password تنظیم کنید

### 2. تنظیم Connection String در Plesk

#### روش 1: Environment Variables (پیشنهادی)

1. **Websites & Domains** > **Hosting Settings**
2. در بخش **Environment Variables** موارد زیر را اضافه کنید:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=localhost;Database=PardisAcademy;User Id=db_user;Password=db_password;TrustServerCertificate=True;
JwtSettings__Key=YourProductionJWTKey2024!@#$%^&*()
EmailSettings__SmtpHost=smtp.gmail.com
EmailSettings__SmtpPort=587
EmailSettings__SmtpUser=your-email@gmail.com
EmailSettings__SmtpPassword=your-app-password
```

#### روش 2: appsettings.Production.json (خودکار via CI/CD)

فایل‌های appsettings که ایجاد کردیم به صورت خودکار در CI/CD پردازش می‌شوند.

## نمونه Connection String برای هاست‌های مختلف

### هاست ایرانی (مثل پارس پک، میزبان فا)

```json
"Server=.\\SQLEXPRESS;Database=username_PardisAcademy;User Id=username_dbuser;Password=your_password;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=True;"
```

### هاست خارجی (مثل GoDaddy، HostGator)

```json
"Server=your-server.database.com;Database=PardisAcademy;User Id=your_username;Password=your_password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### Azure SQL Database

```json
"Server=tcp:your-server.database.windows.net,1433;Initial Catalog=PardisAcademy;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

## تست Connection String

### 1. تست محلی

```bash
# در Visual Studio Package Manager Console
Update-Database -Verbose
```

### 2. تست در سرور

```powershell
# در Plesk File Manager یا SSH
cd /httpdocs/api
dotnet ef database update --verbose
```

### 3. تست از طریق API

```bash
# Health Check
curl https://your-domain.com/api/health

# Database Check
curl https://your-domain.com/api/health/ready
```

## عیب‌یابی مشکلات رایج

### خطای "Login failed for user"

```json
// بررسی کنید:
1. نام کاربری و رمز عبور صحیح باشد
2. User دسترسی به Database داشته باشد
3. SQL Server Authentication فعال باشد
```

### خطای "Server not found"

```json
// بررسی کنید:
1. نام سرور صحیح باشد
2. Port درست باشد (معمولاً 1433)
3. Firewall مشکلی نداشته باشد
```

### خطای "Database not found"

```json
// بررسی کنید:
1. نام Database صحیح باشد
2. Database ایجاد شده باشد
3. User دسترسی به Database داشته باشد
```

### خطای SSL/TLS

```json
// اضافه کنید:
"TrustServerCertificate=True;"
// یا
"Encrypt=False;"
```

## بهترین شیوه‌ها

### 1. امنیت

- هرگز Connection String را در کد commit نکنید
- از Environment Variables استفاده کنید
- رمزهای قوی استفاده کنید
- SSL/TLS فعال کنید

### 2. Performance

- Connection Pooling فعال کنید
- Timeout مناسب تنظیم کنید
- MultipleActiveResultSets در صورت نیاز

### 3. Monitoring

- Connection String را در logs نمایش ندهید
- Health Check endpoints تنظیم کنید
- Database performance monitor کنید

## مثال کامل تنظیمات

### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server.database.com;Database=PardisAcademy;User Id=prod_user;Password=SecurePassword123!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=true;"
  },
  "JwtSettings": {
    "Key": "YourSuperSecretProductionJWTKey2024!@#$%^&*()",
    "Issuer": "PardisAcademy",
    "Audience": "PardisAcademyUsers"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "noreply@pardistous.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@pardistous.com",
    "FromName": "آکادمی پردیس توس"
  }
}
```

### Environment Variables در Plesk

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
```

این راهنما تمام جنبه‌های تنظیم Connection String را پوشش می‌دهد و مشکلات رایج را حل می‌کند.
