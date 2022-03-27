cd %~dp0

cd ..
call clean.bat
call build.cmd
cd NuGet

del *.nupkg
del *.snupkg

nuget pack ModbusMaster.nuspec

pause

