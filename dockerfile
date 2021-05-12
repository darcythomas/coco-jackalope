FROM mcr.microsoft.com/dotnet/sdk:latest

VOLUME /src
VOLUME /build

# restore as a separate layer to speed up builds
WORKDIR /src
COPY src/frontend/frontend.csproj .
RUN dotnet restore

COPY src/frontend/ .
CMD  dotnet publish -c Release -o /build/out/ 