#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/nightly/aspnet:6.0-jammy-chiseled AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build
WORKDIR /src
COPY ["4.Presentations/App.Web/App.Web.csproj", "4.Presentations/App.Web/"]
RUN dotnet restore "4.Presentations/App.Web/App.Web.csproj"
COPY . .
WORKDIR "/src/4.Presentations/App.Web"
RUN dotnet build "App.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Web.dll"]