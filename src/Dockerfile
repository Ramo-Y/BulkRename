FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0.0

COPY ./BulkRename/BulkRename.csproj /build/
COPY ./Directory.Build.props /build/

RUN dotnet restore ./build/BulkRename.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./BulkRename/BulkRename.csproj -c ${BUILDCONFIG} -o out /p:version=${VERSION}

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine
WORKDIR /app
EXPOSE 8080
COPY --from=build /build/out .
ENTRYPOINT ["dotnet", "BulkRename.dll"]