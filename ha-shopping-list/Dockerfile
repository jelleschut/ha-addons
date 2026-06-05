FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/*.sln ./
COPY src/HAShoppingList.Web/*.csproj HAShoppingList.Web/
COPY src/HAShoppingList.Core/*.csproj HAShoppingList.Core/
COPY src/HAShoppingList.Infrastructure/*.csproj HAShoppingList.Infrastructure/
RUN dotnet restore

COPY src/ .
RUN dotnet publish HAShoppingList.Web/HAShoppingList.Web.csproj \
    -c Release \
    -r linux-arm64 \
    --self-contained false \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV Database__Path=/share/shoppinglist.db
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HAShoppingList.Web.dll"]
