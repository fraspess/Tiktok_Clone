# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]

COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["VideoProcessor/VideoProcessor.csproj", "VideoProcessor/"]
RUN dotnet restore "VideoProcessor/VideoProcessor.csproj"
COPY . .
WORKDIR /src/VideoProcessor
RUN dotnet publish "VideoProcessor.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN apt-get update && \
    apt-get install -y --no-install-recommends ffmpeg \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VideoProcessor.dll"]