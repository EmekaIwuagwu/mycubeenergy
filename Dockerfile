# Use the official .NET Core SDK image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

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

# Use the official .NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Set the working directory
WORKDIR /app

# Copy the published output from the build image
COPY --from=build /app/out .

# Set the entry point
ENTRYPOINT ["dotnet", "CubeEnergy.dll"]
