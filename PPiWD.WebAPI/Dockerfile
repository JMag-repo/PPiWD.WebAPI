FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PPiWD.WebAPI/PPiWD.WebAPI.csproj", "PPiWD.WebAPI/"]
RUN dotnet restore "PPiWD.WebAPI/PPiWD.WebAPI.csproj"
COPY . .
WORKDIR "/src/PPiWD.WebAPI"
RUN dotnet build "PPiWD.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PPiWD.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PPiWD.WebAPI.dll"]
