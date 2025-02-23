# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Copy the solution file (if you have one) and restore dependencies
WORKDIR /src
COPY ["src/Authorization.API/" , "Authorization.API/"]
COPY ["src/Authorization.Application/" , "Authorization.Application/"]
COPY ["src/Authorization.Domain/" , "Authorization.Domain/"]
COPY ["src/Authorization.Infrastructure/" , "Authorization.Infrastructure/"]

WORKDIR /src/Authorization.API/
RUN dotnet restore "./Authorization.API.csproj"
RUN dotnet build "./Authorization.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/build


RUN dotnet publish "./Authorization.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT [ "dotnet", "Authorization.API.dll" ]