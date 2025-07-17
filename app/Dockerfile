FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /build

COPY ./Stackage.OAuth2.Fake/Stackage.OAuth2.Fake.csproj ./Stackage.OAuth2.Fake/Stackage.OAuth2.Fake.csproj
COPY ./Stackage.OAuth2.Fake.Tests/Stackage.OAuth2.Fake.Tests.csproj ./Stackage.OAuth2.Fake.Tests/Stackage.OAuth2.Fake.Tests.csproj

RUN dotnet restore ./Stackage.OAuth2.Fake/Stackage.OAuth2.Fake.csproj
RUN dotnet restore ./Stackage.OAuth2.Fake.Tests/Stackage.OAuth2.Fake.Tests.csproj

COPY . .

RUN dotnet publish --configuration Release --no-restore ./Stackage.OAuth2.Fake/Stackage.OAuth2.Fake.csproj

RUN dotnet test --configuration Release --no-restore ./Stackage.OAuth2.Fake.Tests/Stackage.OAuth2.Fake.Tests.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

COPY --from=build /build/Stackage.OAuth2.Fake/bin/Release/net8.0/publish .

# Using the exec form of CMD allows SIGINT to be passed from container to app
CMD ["dotnet", "Stackage.OAuth2.Fake.dll"]
