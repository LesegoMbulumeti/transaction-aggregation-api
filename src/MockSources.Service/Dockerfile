# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY src/MockSources.Service/MockSources.Service.csproj ./src/MockSources.Service/
RUN dotnet restore ./src/MockSources.Service/MockSources.Service.csproj

# Now copy the source and build
COPY src/MockSources.Service/. ./src/MockSources.Service/
WORKDIR /src/src/MockSources.Service
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MockSources.Service.dll"]