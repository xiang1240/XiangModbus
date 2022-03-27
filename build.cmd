cd /d "%~dp0"

set MSBuildFolder=D:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin
"%MSBuildFolder%\MSBuild.exe" XiangModbus.sln /p:Configuration=Release  /t:Restore;Rebuild /v:m
