cd /d "%~dp0"

set MSBuildFolder=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin

"%MSBuildFolder%\MSBuild.exe" XiangModbus.sln /p:Configuration=Release  /t:Restore;Rebuild /v:m
