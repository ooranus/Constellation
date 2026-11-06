@echo off
title CONSTELLATION - Compile
taskkill /F /IM Constellation.exe >nul 2>&1
set CSC=
if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if exist "C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" set CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
if exist "C:\Windows\Microsoft.NET\Framework64\v2.0.50727\csc.exe" set CSC=C:\Windows\Microsoft.NET\Framework64\v2.0.50727\csc.exe
if exist "C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe" set CSC=C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe
if "%CSC%"=="" echo csc.exe not found, install .NET Framework & pause & exit
"%CSC%" /target:winexe /out:Constellation.exe Constellation.cs
if errorlevel 1 echo COMPILE FAILED & pause & exit
echo OK - Constellation.exe built
start Constellation.exe
