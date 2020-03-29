ARG aspnet_tag=3.1
ARG node_tag=13.12.0
ARG sdk_tag=3.1.201

FROM mcr.microsoft.com/dotnet/core/aspnet:${aspnet_tag} AS base
WORKDIR /app
EXPOSE 80

FROM node:${node_tag} as clinetApp
WORKDIR /src
COPY src/TimeManager.Web/ClientApp/ ./ClientApp/
WORKDIR /src/ClientApp
RUN npm i
RUN npm run build

FROM mcr.microsoft.com/dotnet/core/sdk:${sdk_tag} AS build
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