FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Prism.ProAssistant.Api/Prism.ProAssistant.Api.csproj", "Prism.ProAssistant.Api/"]
RUN dotnet restore "Prism.ProAssistant.Api/Prism.ProAssistant.Api.csproj"
COPY . .
WORKDIR "/src/Prism.ProAssistant.Api"
RUN dotnet build "Prism.ProAssistant.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Prism.ProAssistant.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Prism.ProAssistant.Api.dll"]
