FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILDCONFIG=Release
ARG VERSION=1.0.0.0

WORKDIR /build

COPY ./BulkRename/BulkRename.csproj .
COPY ./Directory.Build.props .
COPY ./Directory.Packages.props .

RUN dotnet restore ./BulkRename.csproj

COPY . .
RUN dotnet publish ./BulkRename.csproj \
    -c ${BUILDCONFIG} \
    -o /app/out \
    /p:Version=${VERSION} \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && groupadd -r appgroup \
    && useradd -r -g appgroup appuser \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/out ./
RUN mkdir -p /app/Files /app/Logs /app/RenamingHistory \
    && chown -R appuser:appgroup /app
USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "BulkRename.dll"]
