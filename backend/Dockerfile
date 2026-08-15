# Build context is the `backend/` folder (matches Render's Root Directory setting).

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy just the project files first so `dotnet restore` is cached unless a .csproj actually changes.
COPY QrBin.Api/QrBin.Api.csproj QrBin.Api/
COPY QrBin.Data/QrBin.Data.csproj QrBin.Data/
COPY QrBin.Validators/QrBin.Validators.csproj QrBin.Validators/
COPY QrBin.ViewModels/QrBin.ViewModels.csproj QrBin.ViewModels/
RUN dotnet restore QrBin.Api/QrBin.Api.csproj

COPY . .
RUN dotnet publish QrBin.Api/QrBin.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Render assigns the external port via $PORT at container start — bind Kestrel to it directly
# rather than hardcoding 8080, since Render doesn't always use that value.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet QrBin.Api.dll"]
