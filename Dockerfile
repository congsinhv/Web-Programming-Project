# Stage 1: Build the .NET application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Stage 2: Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Install PostgreSQL client
RUN apt-get update && apt-get install -y postgresql-client

# Set environment variables for connecting to the PostgreSQL server
ENV ASPNETCORE_URLS=http://+:80
ENV ConnectionStrings__TodoContext="Host=postgres;Port=5432;Database=todo_db;Username=postgres;Password=Password@1"

# Copy the entrypoint script
COPY entrypoint.sh /app/entrypoint.sh

# Make the script executable
RUN chmod +x /app/entrypoint.sh

# Expose port
EXPOSE 80
# Start the application
ENTRYPOINT ["/app/entrypoint.sh", "dotnet", "app.dll"]