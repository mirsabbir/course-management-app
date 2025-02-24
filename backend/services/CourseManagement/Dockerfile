# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Copy the solution file (if you have one) and restore dependencies
WORKDIR /src
COPY ["src/CourseManagement.API/" , "CourseManagement.API/"]
COPY ["src/CourseManagement.Application/" , "CourseManagement.Application/"]
COPY ["src/CourseManagement.Domain/" , "CourseManagement.Domain/"]
COPY ["src/CourseManagement.Infrastructure/" , "CourseManagement.Infrastructure/"]

WORKDIR /src/CourseManagement.API/
RUN dotnet restore "./CourseManagement.API.csproj"
RUN dotnet build "./CourseManagement.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

RUN dotnet publish "./CourseManagement.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT [ "dotnet", "CourseManagement.API.dll" ]
