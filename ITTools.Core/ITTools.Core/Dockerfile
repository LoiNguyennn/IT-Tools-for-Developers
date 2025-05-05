# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base stage for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80  
EXPOSE 443  

# Build stage for compiling the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ITTools.Core.csproj", "."]
RUN dotnet restore "./ITTools.Core.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./ITTools.Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage for creating the production artifacts
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ITTools.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create data and Plugins directories, set permissions
RUN mkdir -p /app/data /app/Plugins
COPY --chown=$APP_UID:$APP_UID ITTools.db /app/data/  
# Set permissions for directories and file
RUN chmod -R 777 /app/data /app/Plugins /app/wwwroot/assets
RUN chmod 666 /app/data/ITTools.db || true  

# Switch to non-root user
USER $APP_UID

# Environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/ITTools.db"
ENV Authentication__Google__ClientId="720860860736-abf7igp5tl91rbh91lqeps6pn15aq9p3.apps.googleusercontent.com"
ENV Authentication__Google__ClientSecret="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"


# Entry point
ENTRYPOINT ["dotnet", "ITTools.Core.dll"]