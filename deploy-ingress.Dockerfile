FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
ARG LogsDb__Host
ARG LogsDb__Port
ARG LogsDb__Database
ARG LogsDb__Username
ARG LogsDb__Password
ARG LogsDb__SslMode
ARG LogsDb__CaSert
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./pocket.Logs.Ingress/pocket.Logs.Ingress.csproj"

FROM build AS publish
RUN dotnet publish "./pocket.Logs.Ingress/pocket.Logs.Ingress.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pocket.Logs.Ingress.dll"]
