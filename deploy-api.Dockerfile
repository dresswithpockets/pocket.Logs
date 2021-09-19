FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
ARG LogsDbHost
ARG LogsDbPort
ARG LogsDbDatabase
ARG LogsDbUsername
ARG LogsDbPassword
ARG LogsDbSslMode
ARG LogsDbCaCert
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV LogsDb__Host=${LogsDbHost}
ENV LogsDb__Port=${LogsDbPort}
ENV LogsDb__Database=${LogsDbDatabase}
ENV LogsDb__Username=${LogsDbUsername}
ENV LogsDb__Password=${LogsDbPassword}
ENV LogsDb__SslMode=${LogsDbSslMode}
ENV LogsDb__CaCert=${LogsDbCaCert}

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./pocket.Logs.Api/pocket.Logs.Api.csproj"

FROM build AS publish
RUN dotnet publish "./pocket.Logs.Api/pocket.Logs.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pocket.Logs.Api.dll"]
