#!/bin/sh
ASPNETCORE_URLS=http://+:5124 dotnet /app/api/WakeWeather.Api.dll &
ASPNETCORE_URLS=http://+:8081 dotnet /app/web/WakeWeather.dll
