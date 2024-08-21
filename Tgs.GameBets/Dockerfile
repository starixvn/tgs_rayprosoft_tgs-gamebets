FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameBets/Tgs.GameBets.csproj", "Tgs.GameBets/"]
COPY ["Tgs.GameBets.Common/Tgs.GameBets.Common.csproj", "Tgs.GameBets.Common/"]
COPY ["Tgs.GameBets.Core/Tgs.GameBets.Core.csproj", "Tgs.GameBets.Core/"]
COPY ["Tgs.GameBets.Repositories/Tgs.GameBets.Repositories.csproj", "Tgs.GameBets.Repositories/"]
RUN dotnet restore "./Tgs.GameBets/./Tgs.GameBets.csproj"
COPY . .
WORKDIR "/src/Tgs.GameBets"
RUN dotnet build "./Tgs.GameBets.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameBets.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameBets.dll"]