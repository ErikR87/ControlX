#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ControlX.Hub/ControlX.Hub.csproj", "ControlX.Hub/"]
RUN dotnet restore "ControlX.Hub/ControlX.Hub.csproj"
COPY . .
WORKDIR "/src/ControlX.Hub"
RUN dotnet build "ControlX.Hub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ControlX.Hub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControlX.Hub.dll"]