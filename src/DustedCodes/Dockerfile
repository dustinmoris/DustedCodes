FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build

ARG version=0.0.0-undefined

WORKDIR /app

# Copy everything and build
COPY src/ ./
RUN dotnet publish /p:Version=$version DustedCodes/DustedCodes.fsproj -c Release -o published

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

# Install gRPC prerequisites on Alpine Linux:
# https://github.com/googleapis/google-cloud-dotnet/issues/4780

# Follow thread for proper fix:
# https://github.com/grpc/grpc/issues/21446
RUN echo 'http://dl-cdn.alpinelinux.org/alpine/v3.8/main' >> /etc/apk/repositories && \
    apk update --no-cache && \
    apk add --no-cache bash libc6-compat=1.1.19-r11

WORKDIR /app
COPY --from=build /app/published .
ENTRYPOINT ["dotnet", "DustedCodes.dll"]