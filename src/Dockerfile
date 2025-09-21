FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ShoppingBasket.Application/ShoppingBasket.Application.csproj", "ShoppingBasket.Application/"]
COPY ["ShoppingBasket.Api/ShoppingBasket.Api.csproj", "ShoppingBasket.Api/"]
RUN ls ./ShoppingBasket.Api
RUN dotnet restore "./ShoppingBasket.Api/ShoppingBasket.Api.csproj"
COPY ./ShoppingBasket.Application ./ShoppingBasket.Application
COPY ./ShoppingBasket.Api ./ShoppingBasket.Api
WORKDIR "/src/ShoppingBasket.Api"
RUN dotnet build "./ShoppingBasket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "./ShoppingBasket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine 
RUN apk update && apk upgrade --no-cache && apk add tzdata --no-cache
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ShoppingBasket.Api.dll"]