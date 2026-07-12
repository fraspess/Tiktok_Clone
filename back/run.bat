@echo off
setlocal


docker compose down -v
if errorlevel 1 goto :error

docker compose up -d --build
if errorlevel 1 goto :error

make create-bucket
if errorlevel 1 goto :error

echo.
echo All commands completed successfully.
exit /b 0

:error
echo.
echo A command failed with exit code %errorlevel%.
pause
exit /b %errorlevel%
