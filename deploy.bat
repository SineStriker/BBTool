@echo off

set CURRENT_DIR=%cd%

cd /D %~dp0\BBTool.Net

set RELEASE_DIR=%USERPROFILE%\Desktop\BBTool

set RELEASE_PREFIX=bin\Release\net7.0

set CAMSG_DIR=camsg

set SOMSG_DIR=somsg

set BBRSM_CLI_DIR=BBRsm\BBRsm.Controller

set BBRSM_SVC_DIR=BBRsm\BBRsm.Daemon

mkdir %RELEASE_DIR%

for %%a in (%CAMSG_DIR% %SOMSG_DIR% %BBRSM_CLI_DIR% %BBRSM_SVC_DIR%) do (

    copy %%a\%RELEASE_PREFIX%\*.exe %RELEASE_DIR%

    copy %%a\%RELEASE_PREFIX%\*.dll %RELEASE_DIR%

    copy %%a\%RELEASE_PREFIX%\*.runtimeconfig.json %RELEASE_DIR%

)

cd /D %CURRENT_DIR%