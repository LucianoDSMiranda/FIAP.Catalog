# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FIAP.Catalog.API/FIAP.Catalog.API.csproj", "FIAP.Catalog.API/"]
COPY ["FIAP.Catalog.Application/FIAP.Catalog.Application.csproj", "FIAP.Catalog.Application/"]
COPY ["FIAP.Catalog.Domain/FIAP.Catalog.Domain.csproj", "FIAP.Catalog.Domain/"]
COPY ["FIAP.Catalog.Infrastructure/FIAP.Catalog.Infrastructure.csproj", "FIAP.Catalog.Infrastructure/"]

RUN dotnet restore "FIAP.Catalog.API/FIAP.Catalog.API.csproj"

COPY . .

WORKDIR "/src/FIAP.Catalog.API"

RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FIAP.Catalog.API.dll"]
