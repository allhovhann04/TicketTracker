# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first, using only the project file, so dependency restore is cached
# separately from source changes.
COPY TicketTracker.Api/TicketTracker.Api.csproj TicketTracker.Api/
RUN dotnet restore TicketTracker.Api/TicketTracker.Api.csproj

COPY TicketTracker.Api/ TicketTracker.Api/
RUN dotnet publish TicketTracker.Api/TicketTracker.Api.csproj -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000

ENTRYPOINT ["dotnet", "TicketTracker.Api.dll"]
