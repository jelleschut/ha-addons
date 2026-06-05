FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/WakeWeather.sln ./
COPY src/WakeWeather.Core/*.csproj WakeWeather.Core/
COPY src/WakeWeather.Api/*.csproj WakeWeather.Api/
COPY src/WakeWeather/*.csproj WakeWeather/
RUN dotnet restore

COPY src/WakeWeather.Core/ WakeWeather.Core/
COPY src/WakeWeather.Api/ WakeWeather.Api/
COPY src/WakeWeather/ WakeWeather/

RUN dotnet publish WakeWeather.Api/WakeWeather.Api.csproj \
    -c Release \
    -r linux-arm64 \
    --self-contained false \
    -o /app/api

RUN dotnet publish WakeWeather/WakeWeather.csproj \
    -c Release \
    -r linux-arm64 \
    --self-contained false \
    -o /app/web

# Angular build — uncomment when WakeWeather.Angular is ready to deploy:
# FROM node:22-alpine AS angular-build
# WORKDIR /ng
# COPY src/WakeWeather.Angular/package*.json ./
# RUN npm ci
# COPY src/WakeWeather.Angular/ .
# RUN npx ng build --output-path /app/angular

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/api ./api
COPY --from=build /app/web ./web
COPY startup.sh ./startup.sh
RUN chmod +x startup.sh

ENV ConnectionStrings__DefaultConnection="Data Source=/share/wakeweather.db"
ENV ApiBaseUrl="http://localhost:5124"

ENTRYPOINT ["./startup.sh"]
