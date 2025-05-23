FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src


COPY *.sln .
COPY MetaExchange.WebAPI/*.csproj MetaExchange.WebAPI/
COPY MetaExchange.Application/*.csproj MetaExchange.Application/
COPY MetaExchange.Domain/*.csproj MetaExchange.Domain/
COPY MetaExchange.Infrastructure/*.csproj MetaExchange.Infrastructure/
COPY MetaExchange.SharedKernel/*.csproj MetaExchange.SharedKernel/


RUN dotnet restore


COPY . .


WORKDIR /src/MetaExchange.WebAPI
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app


COPY --from=build /app/publish .


COPY Data /app/Data

ENTRYPOINT ["dotnet", "MetaExchange.WebAPI.dll"]
