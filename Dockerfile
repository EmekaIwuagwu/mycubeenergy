# Use the official .NET 8.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project file and restore dependencies
COPY CubeEnergy.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Build the project
RUN dotnet build -c Release

# Publish the application
RUN dotnet publish -c Release -o out

# Use the official .NET 8.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the published output from the build image
COPY --from=build /app/out .

# Expose port 80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "CubeEnergy.dll"]
