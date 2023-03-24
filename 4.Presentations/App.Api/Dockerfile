#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/nightly/aspnet:6.0-jammy-chiseled AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build
WORKDIR /src
COPY ["4.Presentations/App.Api/App.Api.csproj", "4.Presentations/App.Api/"]
COPY ["3.Applications/App.Application/App.Application.csproj", "3.Applications/App.Application/"]
COPY ["2.Infra/App.Database/App.Database.csproj", "2.Infra/App.Database/"]
COPY ["1.Core/App.Core/App.Core.csproj", "1.Core/App.Core/"]
RUN dotnet restore "4.Presentations/App.Api/App.Api.csproj"
COPY . .
WORKDIR "/src/4.Presentations/App.Api"
RUN dotnet build "App.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Api.dll"]