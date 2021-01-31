FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build

ARG version=0.0.0-undefined

WORKDIR /app

# Copy everything and build
COPY src/ ./
RUN dotnet publish /p:Version=$version DustedCodes/DustedCodes.fsproj -c Release -o published

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

# Install gRPC prerequisites:
RUN apt-get update && apt-get install -y libc6-dev && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/published .
ENTRYPOINT ["dotnet", "DustedCodes.dll"]