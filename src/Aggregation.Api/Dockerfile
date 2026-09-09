# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project file firstand skip it on rebuilds unless dependencies actually change.
COPY src/Aggregation.Api/Aggregation.Api.csproj ./src/Aggregation.Api/
RUN dotnet restore ./src/Aggregation.Api/Aggregation.Api.csproj

# Now copy the rest of the source and build
COPY src/Aggregation.Api/. ./src/Aggregation.Api/
WORKDIR /src/src/Aggregation.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Aggregation.Api.dll"]