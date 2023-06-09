#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8002

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DoorWebAPI/DoorWebAPI.csproj", "DoorWebAPI/"]
COPY ["RabbitMQService/RabbitMQServiceLib.csproj", "RabbitMQService/"]
RUN dotnet restore "DoorWebAPI/DoorWebAPI.csproj"
COPY . .
WORKDIR "/src/DoorWebAPI"
RUN dotnet build "DoorWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoorWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoorWebAPI.dll"]