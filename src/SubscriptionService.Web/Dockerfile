FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SubscriptionService.Web/SubscriptionService.Web.csproj", "src/SubscriptionService.Web/"]
COPY ["src/SubscriptionService.Application/SubscriptionService.Application.csproj", "src/SubscriptionService.Application/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
COPY ["src/SubscriptionService.Domain/SubscriptionService.Domain.csproj", "src/SubscriptionService.Domain/"]
COPY ["src/SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj", "src/SubscriptionService.Infrastructure/"]
RUN dotnet restore "src/SubscriptionService.Web/SubscriptionService.Web.csproj"
COPY . .
WORKDIR "/src/src/SubscriptionService.Web"
RUN dotnet build "./SubscriptionService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SubscriptionService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriptionService.Web.dll"]
