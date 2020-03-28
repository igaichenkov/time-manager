#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM node:13.12.0 as clinetApp
WORKDIR /src
COPY src/TimeManager.Web/ClientApp/ ./ClientApp/
WORKDIR /src/ClientApp
RUN ls
RUN npm i
RUN npm run build
RUN ls

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201 AS build
WORKDIR /src
COPY ["src/TimeManager.Web/TimeManager.Web.csproj", "src/TimeManager.Web/"]
RUN dotnet restore "src/TimeManager.Web/TimeManager.Web.csproj"
COPY . .
WORKDIR "/src/src/TimeManager.Web"
RUN dotnet build "TimeManager.Web.csproj" -c DockerRelease -o /app/build

FROM build AS publish
RUN dotnet publish "TimeManager.Web.csproj" -c DockerRelease -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=clinetApp /src/ClientApp/build ./ClientApp/build
ENTRYPOINT ["dotnet", "TimeManager.Web.dll"]