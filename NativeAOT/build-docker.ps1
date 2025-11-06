# Build Docker image
Write-Host "Building Docker image..." -ForegroundColor Green
docker build -t aot-console-app .

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild successful!" -ForegroundColor Green
    Write-Host "`nTo run the container:" -ForegroundColor Yellow
    Write-Host "  docker run --rm aot-console-app" -ForegroundColor Cyan
    Write-Host "`nTo run with arguments:" -ForegroundColor Yellow
    Write-Host "  docker run --rm aot-console-app arg1 arg2" -ForegroundColor Cyan
} else {
    Write-Host "`nBuild failed!" -ForegroundColor Red
}
