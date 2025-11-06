# Use .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install dependencies required for NativeAOT on Linux
RUN apt-get update && \
    apt-get install -y clang zlib1g-dev && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy project files
COPY *.csproj ./
COPY *.cs ./

# Restore dependencies
RUN dotnet restore

# Publish the application with NativeAOT
RUN dotnet publish -c Release -o /app/publish

# Runtime stage - use a minimal base image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS runtime

WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Run the application
ENTRYPOINT ["./AotConsoleApp"]
