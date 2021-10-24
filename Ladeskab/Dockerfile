FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ChargingCabinet/ChargingCabinet.csproj", "ChargingCabinet/"]
RUN dotnet restore "ChargingCabinet/ChargingCabinet.csproj"
COPY . .
WORKDIR "/src/ChargingCabinet"
RUN dotnet build "ChargingCabinet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChargingCabinet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChargingCabinet.dll"]
