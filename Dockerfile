FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/FooKit.API/FooKit.API.csproj", "src/FooKit.API/"]
COPY ["src/FooKit.Application/FooKit.Application.csproj", "src/FooKit.Application/"]
COPY ["src/FooKit.Domain/FooKit.Domain.csproj", "src/FooKit.Domain/"]
COPY ["src/FooKit.Infrastructure/FooKit.Infrastructure.csproj", "src/FooKit.Infrastructure/"]
RUN dotnet restore "src/FooKit.API/FooKit.API.csproj"

# Copy the rest of the source code
COPY src/ src/

# Build and publish
WORKDIR "/src/src/FooKit.API"
RUN dotnet publish "FooKit.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FooKit.API.dll"]
