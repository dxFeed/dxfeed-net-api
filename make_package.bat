@echo off
rem

msbuild %~dp0\dxf_master\dxf_master.csproj /m /t:Build /p:Configuration=Release /p:Platform=AnyCPU

