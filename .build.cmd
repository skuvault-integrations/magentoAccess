@echo off
IF %1.==. GOTO No1

setlocal
set PATH=%PATH%;tools\Invoke-Build

set LOGDIR=%~dp0log\
set LOGFILE=%LOGDIR%%1.log
if not exist %LOGDIR% md %LOGDIR%

powershell -NoProfile -ExecutionPolicy unrestricted "Build -Parameters @{Configuration='%1'} -Summary:$True" -verbose

PAUSE
GOTO End1

:No1
	echo .build.cmd cannot be run directly.
	echo Execute other cmd file instead (for example release.cmd).
	pause
GOTO End1

:End1