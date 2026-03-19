# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o csproj
COPY *.csproj ./

# Restore
RUN dotnet restore

# Copia o restante
COPY . .

# Publish
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FIAP.Catalog.dll"]