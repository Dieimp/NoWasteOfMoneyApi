# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NoWasteOfMoney.csproj", "."]
RUN dotnet restore "./NoWasteOfMoney.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./NoWasteOfMoney.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NoWasteOfMoney.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variable for MySQL connection inside the container
ENV ConnectionStrings__DefaultConnection="Server=mysql;Port=3306;Database=NoWasteOfMoney;Uid=root;Pwd=BPdkJqlupAH4VupLKTF7;"

ENTRYPOINT ["dotnet", "NoWasteOfMoney.dll"]
