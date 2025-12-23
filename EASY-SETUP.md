# راهنمای ساده دیپلویمنت

## فقط 3 قدم!

### 1. GitHub Secrets تنظیم کن

در GitHub repository خودت برو به **Settings > Secrets** و اینا رو اضافه کن:

```
FTP_HOST=ftp.pardistous.com
FTP_USER=your_username
FTP_PASSWORD=your_password
DATABASE_CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=PardisAcademy;User Id=db_user;Password=db_pass;TrustServerCertificate=True;
JWT_SECRET_KEY=YourSecretKey123!@#
```

### 2. در Plesk تنظیم کن

#### Database بساز:

- برو **Databases** > **Add Database**
- اسم: `PardisAcademy`
- یه user و password بساز

#### ASP.NET Core فعال کن:

- برو **Hosting Settings**
- **ASP.NET Core support** رو تیک بزن
- **.NET version** رو روی **8.0** بذار

### 3. Push کن!

```bash
git add .
git commit -m "deploy"
git push origin main
```

## اگه مشکل داشتی:

### سایت باز نمیشه:

- چک کن ASP.NET Core نصب باشه
- چک کن فایل‌ها آپلود شده باشن

### API کار نمیکنه:

- Connection String رو چک کن
- Database رو چک کن که وجود داشته باشه

### React routing کار نمیکنه:

- web.config باید در root باشه
- URL Rewrite در IIS فعال باشه

## تست کن:

- برو https://pardistous.com
- برو https://pardistous.com/api/health

همین! 🚀
