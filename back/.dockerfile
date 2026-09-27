
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy this so all projects reference the same c# version
COPY ["Directory.Build.props", "."] 

# Copy this so all projects reference the same package versions
COPY ["Directory.Packages.props", "."]

# Copy all .csproj files and restore
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["Api/Api.csproj", "Api/"]

RUN dotnet restore "Api/Api.csproj"

# Copy everything and publish
COPY . .
WORKDIR /src/Api
RUN dotnet publish "Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install FFmpeg & curl
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        curl \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy published output
COPY --from=build /app/publish .


ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]