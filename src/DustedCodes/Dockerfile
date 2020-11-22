FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copy everything and build
COPY src/ ./
RUN dotnet publish DustedCodes/DustedCodes.fsproj -c Release -o published

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/published .
ENV ASPNETCORE_URLS http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DustedCodes.dll"]