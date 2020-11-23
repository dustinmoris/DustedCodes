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
ENV LD_LIBRARY_PATH=/app/bin/clidriver/lib:/app/bin/clidriver/lib/libdb2.so
ENV PATH=$PATH:/app/clidriver/bin:/app/clidriver/lib
EXPOSE 8080
ENTRYPOINT ["dotnet", "DustedCodes.dll"]