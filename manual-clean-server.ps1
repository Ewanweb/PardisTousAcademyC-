# اسکریپت دستی برای پاک کردن کامل سرور
Write-Host "🚨 اسکریپت پاک‌سازی کامل سرور" -ForegroundColor Red
Write-Host "این اسکریپت تمام فایل‌های سرور رو پاک می‌کنه!" -ForegroundColor Yellow
Write-Host ""

# تأیید از کاربر
$confirmation = Read-Host "آیا مطمئنی که می‌خوای تمام فایل‌های سرور رو پاک کنی؟ (YES/no)"

if ($confirmation -ne "YES") {
    Write-Host "❌ عملیات لغو شد" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "🗑️ شروع پاک‌سازی..." -ForegroundColor Red

# مرحله 1: پاک‌سازی فایل‌های اصلی
Write-Host "مرحله 1: پاک‌سازی فایل‌های اصلی..." -ForegroundColor Yellow

@"
open ftp.api.pardistous.ir
ewan
mahan1384@
binary

# حذف فایل‌های اصلی
delete Api.dll
delete Api.exe
delete Api.pdb
delete Api.deps.json
delete Api.runtimeconfig.json
delete appsettings.json
delete appsettings.Development.json
delete appsettings.Production.json
delete web.config
delete app_offline.htm

# حذف فایل‌های .NET
delete Microsoft.AspNetCore.dll
delete Microsoft.Extensions.dll
delete System.Text.Json.dll
delete Newtonsoft.Json.dll
delete AutoMapper.dll
delete MediatR.dll

# حذف فایل‌های پروژه
delete Pardis.Application.dll
delete Pardis.Domain.dll
delete Pardis.Infrastructure.dll
delete Pardis.Query.dll
delete Pardis.Facade.dll

quit
"@ | Out-File -FilePath "ftp_clean_step1.txt" -Encoding ASCII

try {
    ftp -s:ftp_clean_step1.txt
    Write-Host "✅ مرحله 1 تکمیل شد" -ForegroundColor Green
} catch {
    Write-Host "⚠️ خطا در مرحله 1: $($_.Exception.Message)" -ForegroundColor Red
}

Remove-Item ftp_clean_step1.txt -ErrorAction SilentlyContinue

# مرحله 2: پاک‌سازی عمیق
Write-Host "مرحله 2: پاک‌سازی عمیق..." -ForegroundColor Yellow

@"
open ftp.api.pardistous.ir
ewan
mahan1384@

# حذف تمام فایل‌های dll
delete *.dll
delete *.exe
delete *.pdb
delete *.json
delete *.config
delete *.xml
delete *.htm
delete *.html

quit
"@ | Out-File -FilePath "ftp_clean_step2.txt" -Encoding ASCII

try {
    ftp -s:ftp_clean_step2.txt
    Write-Host "✅ مرحله 2 تکمیل شد" -ForegroundColor Green
} catch {
    Write-Host "⚠️ خطا در مرحله 2: $($_.Exception.Message)" -ForegroundColor Red
}

Remove-Item ftp_clean_step2.txt -ErrorAction SilentlyContinue

# مرحله 3: بررسی نهایی
Write-Host "مرحله 3: بررسی نهایی..." -ForegroundColor Yellow

@"
open ftp.api.pardistous.ir
ewan
mahan1384@
dir
quit
"@ | Out-File -FilePath "ftp_check.txt" -Encoding ASCII

try {
    Write-Host "فایل‌های باقی‌مانده در سرور:" -ForegroundColor Cyan
    ftp -s:ftp_check.txt
} catch {
    Write-Host "⚠️ خطا در بررسی: $($_.Exception.Message)" -ForegroundColor Red
}

Remove-Item ftp_check.txt -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "🎉 پاک‌سازی کامل شد!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "حالا می‌تونی deployment جدید رو اجرا کنی:" -ForegroundColor White
Write-Host "1. برو به GitHub Actions" -ForegroundColor White
Write-Host "2. 'Clean Deploy' رو انتخاب کن" -ForegroundColor White
Write-Host "3. 'Run workflow' رو بزن" -ForegroundColor White
Write-Host "4. در قسمت confirm_clean عبارت 'YES' رو وارد کن" -ForegroundColor White
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan