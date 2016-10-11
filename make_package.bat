@echo off
rem

rem TODO: check MSBuild Extension Pack

setlocal

rem Check msbuild application in PATH
where /q msbuild
if %ERRORLEVEL% GEQ 1 (
    echo The 'msbuild' application is missing. Ensure it is installed and placed in your PATH.
    goto exit_error
)
rem Check mstest application in PATH
rem where /q mstest
rem if %ERRORLEVEL% GEQ 1 (
rem     echo The 'mstest' application is missing. Ensure it is installed and placed in your PATH. For example for Visual Studio 2015 this path is '^<drive^>:\Program Files ^(x86^)\Microsoft Visual Studio 14.0^\Common7^\IDE'
rem     goto exit_error
rem )

rem Check nunit-console application in PATH
where /q nunit-console
if %ERRORLEVEL% GEQ 1 (
    echo The 'nunit-console' application is missing. Ensure it is installed and placed in your PATH.
    goto exit_error
)

set VERSION=%1
set C_API_PATH=%~dp0\%2
set C_API_BUILD=%C_API_PATH%\build.bat

rem Check version parameter
if [%VERSION%] EQU [] (
    echo ERROR: The version of package is not specified or invalid^!
    goto usage
)
rem Check C API path parameter
if [%C_API_PATH%] EQU [] (
    echo ERROR: The c-api-path is not specified or invalid^!
    goto usage
)
if NOT EXIST %C_API_BUILD% (
    echo ERROR: The build script '%C_API_BUILD%' is not exist^!
    goto usage
)

rem === BUILD PROJECTS ===
msbuild %~dp0\dxf_master\dxf_master.csproj /m /t:Clean;UpdateVersion;Build;RunUnitTests;CopySources;CreatePackage /p:Configuration=Release;Platform=AnyCPU;AssemblyVersion=%VERSION%
if %ERRORLEVEL% GEQ 1 goto exit_error

rem === TEST BUILDS ===
rem msbuild %~dp0\dxf_master\dxf_master.csproj /m /t:RunUnitTests /p:Configuration=Release;Platform=AnyCPU;AssemblyVersion=%VERSION%
rem if %ERRORLEVEL% GEQ 1 goto exit_error

rem === MAKE PACKAGE ===

rem === FINISH ===
goto exit_success

:usage
echo.
echo Usage: %0 ^<major.minor.patch^> ^<c-api-path^> [no-test]
echo    ^<major.minor.patch^> - Version of package, i.e. 1.2.6
echo    ^<c-api-path^>        - The path to C API directory where main build.sh is 
echo                          located
echo    no-test             - build tests will not be started
goto exit_error

:exit_success
echo.
echo Making package complete successfully.
exit /b 0
:exit_error
echo.
echo Making package failed^!
exit /b 1
:end