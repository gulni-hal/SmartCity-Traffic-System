FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# proje dosyalarini kopyala ve restore etme alani
COPY ["Dispatcher.Api/Dispatcher.Api.csproj", "Dispatcher.Api/"]
COPY ["Dispatcher.Application/Dispatcher.Application.csproj", "Dispatcher.Application/"]
COPY ["Dispatcher.Infrastructure/Dispatcher.Infrastructure.csproj", "Dispatcher.Infrastructure/"]
RUN dotnet restore "Dispatcher.Api/Dispatcher.Api.csproj"

# tüm kodları kopyala ve derle
COPY . .
WORKDIR "/src/Dispatcher.Api"
RUN dotnet build "Dispatcher.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dispatcher.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dispatcher.Api.dll"]