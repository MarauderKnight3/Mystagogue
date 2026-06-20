@echo off
setlocal enabledelayedexpansion

::   repos/
::       tModLoader/
::           patches/
::               Terraria/     ? source
::       Mystagogue/
::           Terraria/         ? will be deleted + replaced
::           copy_patches.bat  ? this script

set "SCRIPT_DIR=%~dp0"
set "SCRIPT_DIR=%SCRIPT_DIR:~0,-1%"
set "PARENT_DIR=%SCRIPT_DIR%\.."

if not exist "%PARENT_DIR%\tModLoader\patches\Terraria" (
    echo( ..\tModLoader\patches\Terraria not found!
    echo( Make sure tModLoader folder is directly next to this folder.
    pause
    exit /b 1
)

if exist "%SCRIPT_DIR%\Terraria" (
    rmdir /s /q "%SCRIPT_DIR%\Terraria"
)

xcopy "%PARENT_DIR%\tModLoader\patches\Terraria" "%SCRIPT_DIR%\Terraria" /f /s /i /y >nul

if errorlevel 1 (
    echo.
    echo( Copying failed!
    pause
    exit /b 1
)
echo( Done
timeout /t 1 > nul