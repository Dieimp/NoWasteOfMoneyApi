# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# FROM docker.io/microsoft-dotnet-aspnet:9.0 AS base
# WORKDIR /app
# USER $APP_UID
# WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081

# FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["NoWasteOfMoney.csproj", "."]
# RUN dotnet restore "./NoWasteOfMoney.csproj"
# COPY . .
# WORKDIR "/src/."
# RUN dotnet build "./NoWasteOfMoney.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ==============================================================================
# Usando Ubuntu Padrão para evitar bloqueios de registros específicos de .NET
# ==============================================================================
FROM ubuntu:22.04 AS final
WORKDIR /app

# Instala os pré-requisitos mínimos para rodar o .NET (caso falte alguma lib nativa)
RUN apt-get update && apt-get install -y libicu-dev libssl-dev && rm -rf /var/lib/apt/lists/*

# Portas padrão
EXPOSE 8080
EXPOSE 8081

# Variáveis de ambiente
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ConnectionStrings__DefaultConnection="Server=mysql;Port=3306;Database=NoWasteOfMoney;Uid=root;Pwd=BPdkJqlupAH4VupLKTF7;"

# Copia os arquivos compilados localmente para dentro do container
COPY ./publish .
RUN chmod +x ./NoWasteOfMoney
# Ponto de entrada chamando o executável que o dotnet publish gera
ENTRYPOINT ["./NoWasteOfMoney"]