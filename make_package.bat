@echo off
rem

rem TODO: check MSBuild Extension Pack

msbuild %~dp0\dxf_master\dxf_master.csproj /m /t:Build /p:Configuration=Release /p:Platform=AnyCPU

