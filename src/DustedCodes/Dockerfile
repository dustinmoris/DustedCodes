FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG version=0.0.0-undefined

WORKDIR /app

# Copy everything and build
COPY src/ ./
RUN dotnet publish /p:Version=$version DustedCodes/DustedCodes.fsproj -c Release -o published

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

WORKDIR /app
COPY --from=build /app/published .
ENTRYPOINT ["dotnet", "DustedCodes.dll"]