FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve restore et
COPY ["Fine.Api/Fine.Api.csproj", "Fine.Api/"]
COPY ["Fine.Application/Fine.Application.csproj", "Fine.Application/"]
COPY ["Fine.Infrastructure/Fine.Infrastructure.csproj", "Fine.Infrastructure/"]
RUN dotnet restore "Fine.Api/Fine.Api.csproj"

# Tüm kodları kopyala ve derle
COPY . .
WORKDIR "/src/Fine.Api"
RUN dotnet build "Fine.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fine.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fine.Api.dll"]