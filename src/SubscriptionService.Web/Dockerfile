FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "./"]
COPY ["src/SubscriptionService.Web/SubscriptionService.Web.csproj", "src/SubscriptionService.Web/"]
COPY ["src/SubscriptionService.Application/SubscriptionService.Application.csproj", "src/SubscriptionService.Application/"]
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/SubscriptionService.Domain/SubscriptionService.Domain.csproj", "src/SubscriptionService.Domain/"]
COPY ["src/SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj", "src/SubscriptionService.Infrastructure/"]
RUN --mount=type=secret,id=github_token,env=GITHUB_TOKEN,required=true \
    env "NuGetPackageSourceCredentials_github-maze37=Username=maze37;Password=${GITHUB_TOKEN};ValidAuthenticationTypes=Basic" \
    dotnet restore "src/SubscriptionService.Web/SubscriptionService.Web.csproj" \
        --configfile "/src/nuget.config"
COPY . .
WORKDIR "/src/src/SubscriptionService.Web"
RUN dotnet publish "./SubscriptionService.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SubscriptionService.Web.dll"]
