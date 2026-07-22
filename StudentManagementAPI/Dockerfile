# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StudentManagementAPI.csproj", "./"]
RUN dotnet restore "StudentManagementAPI.csproj"
COPY . .
RUN dotnet build "StudentManagementAPI.csproj" -c Release -o /app/build
RUN dotnet publish "StudentManagementAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StudentManagementAPI.dll"]
