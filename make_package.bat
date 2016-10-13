@echo off
rem Script builds C API library and configured projects from 
rem dxf_master\dxf_master.csproj. The dxf_master project contains references to
rem all other projects and targets to build.
rem Dependencies:
rem     1. MSBuild 14 
rem     2. MSBuild Command Prompt for VS2015
rem     3. MSBuild Extesion Pack 4.0
rem     4. nunit 2.6
rem     5*. All dependencies from %C_API_PATH%\%C_API_SCRIPT_NAME%
rem The operation order:
rem     1. Build C API library 
rem     2. Copy %C_API_LIB_NAME%(d)(_64)%C_API_LIB_EXT% libraries to lib\* 
rem     folder of current project
rem     3. Build next targets from dxf_master: "Clean;UpdateVersion;Build;
rem     RunUnitTests*;CopySources;GenerateSolution;CreatePackage"
rem     where 
rem         Clean           - Cleanup all projects.
rem         UpdateVersion   - Update assembly version for each project; the 
rem                           version number passed through $(AssemblyVersion) 
rem                           MSBuild parameter.
rem         Build           - Build all projects and copy binaries to 
rem                           bin\dxfeed-net-api-<major.minor.patch>\bin of 
rem                           dxf_master project.
rem         RunUnitTests*   - Build and run unit tests from 
rem                           dxf_tests\dxf_tests.csproj; the list of running 
rem                           tests configured in dxf_master\MakePackageTestList.txt;
rem                           * means that this target can be skipped if input
rem                           script parameter 'no-test' will be specified.
rem         CopySources     - Copy reference project source files to output 
rem                           direcory of dxf_master project.
rem         GenerateSolution- Filter projects, its dependencies and 
rem                           configurations from original solution file and 
rem                           generates new once.
rem         CreatePackage   - Create zip archive containing sources and 
rem                           binaries.
rem
rem Usage: 
rem     make_package <major.minor.patch> <c-api-path> [no-test]
rem Where
rem     <major.minor.patch> - Version of package, i.e. 1.2.6
rem     <c-api-path>        - The path to C API directory where 
rem                           %C_API_SCRIPT_NAME% is located
rem     [no-test]           - build testing will not be started (optional)
rem
rem The result of build is located in dxf_master\bin

setlocal

rem Check msbuild application in PATH
where /q msbuild
if %ERRORLEVEL% GEQ 1 (
    echo The 'msbuild' application is missing. Ensure it is installed and placed in your PATH.
    goto exit_error
)
rem Check nunit-console application in PATH
where /q nunit-console
if %ERRORLEVEL% GEQ 1 (
    echo The 'nunit-console' application is missing. Ensure it is installed and placed in your PATH.
    goto exit_error
)

set VERSION=%1
set C_API_PATH=%~dp0\%2
set C_API_SCRIPT_NAME=make_package.bat
set C_API_BUILD=%C_API_PATH%\%C_API_SCRIPT_NAME%
set C_API_LIB_NAME=DXFeed
set C_API_LIB_EXT=.dll
set TARGET_TEST=RunUnitTests;

for %%A in (%*) do (
    if [%%A] EQU [no-test] set TARGET_TEST=
)

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

rem === BUILD C API ===
set HOME_DIR=%cd%
cd %C_API_PATH%
call %C_API_SCRIPT_NAME% %VERSION% no-test
set C_API_ERRORLEVEL=%ERRORLEVEL%
cd %HOME_DIR%
if %C_API_ERRORLEVEL% GEQ 1 (
    echo C API build failed^!
    goto exit_error
)
xcopy /Y /I %C_API_PATH%\build\x86\Release\%C_API_LIB_NAME%%C_API_LIB_EXT% %~dp0\lib
if %ERRORLEVEL% GEQ 1 goto exit_error
xcopy /Y /I %C_API_PATH%\build\x86\Debug\%C_API_LIB_NAME%d%C_API_LIB_EXT% %~dp0\lib
if %ERRORLEVEL% GEQ 1 goto exit_error
xcopy /Y /I %C_API_PATH%\build\x64\Release\%C_API_LIB_NAME%_64%C_API_LIB_EXT% %~dp0\lib
if %ERRORLEVEL% GEQ 1 goto exit_error
xcopy /Y /I %C_API_PATH%\build\x64\Debug\%C_API_LIB_NAME%d_64%C_API_LIB_EXT% %~dp0\lib
if %ERRORLEVEL% GEQ 1 goto exit_error

rem === BUILD PROJECTS ===
msbuild %~dp0\dxf_master\dxf_master.csproj /m /t:Clean;UpdateVersion;Build;%TARGET_TEST%CopySources;GenerateSolution;CreatePackage /p:Configuration=Release;Platform=AnyCPU;AssemblyVersion=%VERSION%
if %ERRORLEVEL% GEQ 1 goto exit_error

rem === FINISH ===
goto exit_success

:usage
echo.
echo Usage: %0 ^<major.minor.patch^> ^<c-api-path^> [no-test]
echo    ^<major.minor.patch^> - Version of package, i.e. 1.2.6
echo    ^<c-api-path^>        - The path to C API directory where %C_API_SCRIPT_NAME%
echo                          is located
echo    [no-test]           - build tests will not be started (optional)
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