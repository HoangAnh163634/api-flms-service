# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
WORKDIR /app
EXPOSE 5000  # Expose port 5000 instead of 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src

# Copy the .csproj file and restore dependencies
COPY ["api-flms-service.csproj", "./"]
RUN dotnet restore "api-flms-service.csproj"

# Copy the entire project and build
COPY . .
RUN dotnet build "api-flms-service.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "api-flms-service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the environment variable for the ASP.NET Core application to listen on port 5000
ENV ASPNETCORE_URLS=http://+:5000

# Entry point
ENTRYPOINT ["dotnet", "api-flms-service.dll"]
