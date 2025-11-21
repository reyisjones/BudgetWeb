@echo off
REM Budget App Setup Script for Windows
REM This script sets up the development environment for the Budget App

echo.
echo Budget App Setup Script
echo ==========================
echo.

REM Check for .NET
echo Checking for .NET...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET not found!
    echo Please install .NET 10 SDK from: https://dotnet.microsoft.com/download
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo Found .NET version: %DOTNET_VERSION%

REM Check for Node.js
echo.
echo Checking for Node.js...
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Node.js not found!
    echo Please install Node.js from: https://nodejs.org/
    exit /b 1
)

for /f "tokens=*" %%i in ('node --version') do set NODE_VERSION=%%i
echo Found Node.js version: %NODE_VERSION%

REM Install .NET dependencies
echo.
echo Restoring .NET packages...
cd BudgetWeb
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore .NET packages
    exit /b 1
)

REM Install Node.js dependencies
echo.
echo Installing Node.js packages...
cd ClientApp
call npm install
if %errorlevel% neq 0 (
    echo ERROR: Failed to install Node.js packages
    exit /b 1
)

REM Build React app
echo.
echo Building React app...
call npm run build
if %errorlevel% neq 0 (
    echo ERROR: Failed to build React app
    exit /b 1
)

REM Build .NET app
echo.
echo Building .NET app...
cd ..
dotnet build -c Release
if %errorlevel% neq 0 (
    echo ERROR: Failed to build .NET app
    exit /b 1
)

echo.
echo Setup complete!
echo.
echo To run the application:
echo.
echo   Development mode (2 terminals required):
echo   Terminal 1: cd BudgetWeb\ClientApp ^&^& npm start
echo   Terminal 2: cd BudgetWeb ^&^& dotnet run
echo.
echo   Desktop mode:
echo   dotnet tool install ElectronNET.CLI -g
echo   cd BudgetWeb ^&^& electronize start
echo.
echo See README.md for more information.
echo.
pause
