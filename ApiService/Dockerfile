FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ApiService/ApiService.csproj", "ApiService/"]
RUN dotnet restore "ApiService/ApiService.csproj"

COPY . .
WORKDIR "/src/ApiService"
RUN dotnet publish "ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
EXPOSE 8443
ENTRYPOINT ["dotnet", "ApiService.dll"]
