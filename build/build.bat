@echo off
cls
set encoding=utf-8
"..\packages\FAKE.3.14.4\tools\Fake.exe" build.fsx
pause