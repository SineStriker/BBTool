set RELEASE_DIR=build

set CAMSG_DIR=BBTool.Net\camsg\bin\Release\net7.0

set SOMSG_DIR=BBTool.Net\somsg\bin\Release\net7.0

mkdir %RELEASE_DIR%

copy %CAMSG_DIR%\*.exe %RELEASE_DIR%

copy %CAMSG_DIR%\*.dll %RELEASE_DIR%

copy %CAMSG_DIR%\*.runtimeconfig.json %RELEASE_DIR%

copy %SOMSG_DIR%\*.exe %RELEASE_DIR%

copy %SOMSG_DIR%\*.dll %RELEASE_DIR%

copy %SOMSG_DIR%\*.runtimeconfig.json %RELEASE_DIR%