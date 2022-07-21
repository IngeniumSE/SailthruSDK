@echo off

set target=%1
set verbostity=%2
if "%target%" == "" set target=Default
if "%verbostity%" == "" set verbostity=Minimal

cd build\apps\build

dotnet run --target %target% --verbostity %verbostity%