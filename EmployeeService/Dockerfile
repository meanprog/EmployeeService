﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Устанавливаем зависимости
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    curl \
    libgdiplus && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EmployeeService/EmployeeService.csproj", "EmployeeService/"]
RUN dotnet restore "EmployeeService/EmployeeService.csproj"
COPY . .
WORKDIR "/src/EmployeeService"
RUN dotnet build "EmployeeService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeeService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeService.dll"]