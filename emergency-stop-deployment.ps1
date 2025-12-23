# Emergency script to stop stuck deployment
Write-Host "🚨 Emergency Deployment Stop Script" -ForegroundColor Red
Write-Host "This script will attempt to clean up the stuck deployment" -ForegroundColor Yellow

# Create FTP script to remove app_offline.htm if it exists
Write-Host "Creating FTP cleanup script..." -ForegroundColor Cyan

@"
open ftp.api.pardistous.ir
ewan
mahan1384@
delete app_offline.htm
quit
"@ | Out-File -FilePath "cleanup_ftp.txt" -Encoding ASCII

Write-Host "Attempting to remove app_offline.htm via FTP..." -ForegroundColor Cyan

try {
    # Execute FTP command
    ftp -s:cleanup_ftp.txt
    Write-Host "✅ FTP cleanup command executed" -ForegroundColor Green
} catch {
    Write-Host "⚠️ FTP cleanup failed: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Cleanup local files
Remove-Item cleanup_ftp.txt -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "🔧 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Go to GitHub Actions and cancel the stuck workflow manually" -ForegroundColor White
Write-Host "2. Wait a few minutes for the server to recover" -ForegroundColor White
Write-Host "3. Try the simple deployment workflow instead" -ForegroundColor White
Write-Host ""
Write-Host "To run simple deployment:" -ForegroundColor Green
Write-Host "- Go to GitHub Actions" -ForegroundColor White
Write-Host "- Select 'Simple Deploy API to Server' workflow" -ForegroundColor White
Write-Host "- Click 'Run workflow' button" -ForegroundColor White

Write-Host ""
Write-Host "Script completed!" -ForegroundColor Green