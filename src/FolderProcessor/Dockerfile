FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FolderProcessor/FolderProcessor.csproj", "FolderProcessor/"]
RUN dotnet restore "FolderProcessor/FolderProcessor.csproj"
COPY . .
WORKDIR "/src/FolderProcessor"
RUN dotnet build "FolderProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FolderProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FolderProcessor.dll"]
