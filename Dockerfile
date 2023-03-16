# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY aspnetapp/*.csproj ./aspnetapp/
RUN ls aspnetapp
RUN dotnet restore -r linux-x64

# copy everything else and build app
COPY aspnetapp/. ./aspnetapp/
WORKDIR /source/aspnetapp
RUN dotnet publish -c Release -o /app -r linux-x64 --self-contained true --no-restore 

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
# COPY static files
COPY --from=build /source/aspnetapp/www/ ./www/
# set ASPNETCORE_URLS
ENTRYPOINT ["dotnet", "aspnetapp.dll"]