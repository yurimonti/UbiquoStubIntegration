FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
COPY . /src
WORKDIR /src
RUN dotnet restore
COPY . .
RUN dotnet build "UbiquoStub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UbiquoStub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5122
ENTRYPOINT ["dotnet", "UbiquoStub.dll"]