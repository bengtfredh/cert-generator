FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
MAINTAINER Bengt Fredh <brf@digdir.no>


RUN git clone https://github.com/Altinn/cert-generator.git && cd cert-generator && \
    dotnet build Generator.sln

FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS final
MAINTAINER Bengt Fredh <brf@digdir.no>

COPY --from=build cert-generator/src/cert-generator/bin/Debug/netcoreapp3.1/* /usr/local/bin/

WORKDIR /data
VOLUME /data
