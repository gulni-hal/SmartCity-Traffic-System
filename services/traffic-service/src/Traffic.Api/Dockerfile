FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve restore et
COPY ["Traffic.Api/Traffic.Api.csproj", "Traffic.Api/"]
COPY ["Traffic.Application/Traffic.Application.csproj", "Traffic.Application/"]
COPY ["Traffic.Infrastructure/Traffic.Infrastructure.csproj", "Traffic.Infrastructure/"]
RUN dotnet restore "Traffic.Api/Traffic.Api.csproj"

# Tüm kodları kopyala ve derle
COPY . .
WORKDIR "/src/Traffic.Api"
RUN dotnet build "Traffic.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Traffic.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Traffic.Api.dll"]