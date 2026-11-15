FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/AgendaContactos.Api/AgendaContactos.Api.csproj src/AgendaContactos.Api/
RUN dotnet restore src/AgendaContactos.Api/AgendaContactos.Api.csproj

COPY . .
RUN dotnet publish src/AgendaContactos.Api/AgendaContactos.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT [ "sh", "-c", "dotnet AgendaContactos.Api.dll --urls http://0.0.0.0:${PORT:-8080}" ]