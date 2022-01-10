FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/FolderProcessor.Host/FolderProcessor.Host.csproj", "FolderProcessor.Host/"]
RUN dotnet restore "src/FolderProcessor.Host/FolderProcessor.Host.csproj"
COPY . .
WORKDIR "/src/FolderProcessor.Host"
RUN dotnet build "FolderProcessor.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FolderProcessor.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FolderProcessor.Host.dll"]
