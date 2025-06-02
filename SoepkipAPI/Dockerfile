FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SoepkipAPI.csproj", "./"]
RUN dotnet restore "SoepkipAPI.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./SoepkipAPI.csproj" -c $BUILD_CONFIGURATION /p:UseAppHost=false


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SoepkipAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SoepkipAPI.dll"]
