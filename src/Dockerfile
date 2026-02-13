FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY ImmCheck.sln .
COPY ImmCheck.Core/ImmCheck.Core.csproj ImmCheck.Core/
COPY ImmCheck.Infrastructure/ImmCheck.Infrastructure.csproj ImmCheck.Infrastructure/
COPY ImmCheck.Api/ImmCheck.Api.csproj ImmCheck.Api/
RUN dotnet restore

# Copy all source and publish
COPY . .
RUN dotnet publish ImmCheck.Api/ImmCheck.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "ImmCheck.Api.dll"]
