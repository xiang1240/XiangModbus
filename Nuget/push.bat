
nuget push *.nupkg -source https://api.nuget.org/v3/index.json -ApiKey oy2grrnjz4xuhwtzezwq7jkhdtcokc7u7l64mbafp3kfba
::nuget push ModbusMaster.1.0.0.nupkg -source https://api.nuget.org/v3/index.json -ApiKey oy2grrnjz4xuhwtzezwq7jkhdtcokc7u7l64mbafp3kfba
::del *.nupkg
::del *.snupkg

pause
