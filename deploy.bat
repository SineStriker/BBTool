@echo off

set RELEASE_DIR=%USERPROFILE%\Desktop\BBTool

set RELEASE_PREFIX=bin\Release\net7.0

set CAMSG_DIR=BBTool.Net\camsg\%RELEASE_PREFIX%

set SOMSG_DIR=BBTool.Net\somsg\%RELEASE_PREFIX%

set BBRSM_CLI_DIR=BBTool.Net\BBRsm\BBRsm.Controller\%RELEASE_PREFIX%

set BBRSM_SVC_DIR=BBTool.Net\BBRsm\BBRsm.Daemon\%RELEASE_PREFIX%

mkdir %RELEASE_DIR%

for %%a in (%CAMSG_DIR% %SOMSG_DIR% %BBRSM_CLI_DIR% %BBRSM_SVC_DIR%) do (

    copy %%a\*.exe %RELEASE_DIR%

    copy %%a\*.dll %RELEASE_DIR%

    copy %%a\*.runtimeconfig.json %RELEASE_DIR%

)
