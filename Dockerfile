FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
ARG DATABASE_URL
ARG LogsDbSslMode
ARG CA_CERT
WORKDIR /app
EXPOSE 5000

RUN echo ${LogsDbUrl}

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DATABASE_URL=postgresql://doadmin:show-password@db-postgresql-nyc3-29625-do-user-8809852-0.b.db.ondigitalocean.com:25060/defaultdb?sslmode=require
ENV LogsDb__SslMode=require
ENV CA_CERT=${CA_CERT}

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
ARG DATABASE_URL
ENV DATABASE_URL=postgresql://doadmin:show-password@db-postgresql-nyc3-29625-do-user-8809852-0.b.db.ondigitalocean.com:25060/defaultdb?sslmode=require
ARG CA_CERT
ENV CA_CERT=${CA_CERT}
ARG name
ENV name=${name}
WORKDIR /app
COPY --from=publish /app/publish .
#CMD echo $DATABASE_URL && echo $name && echo $CA_CERT
ENTRYPOINT ["dotnet", "pocket.Logs.Api.dll"]
