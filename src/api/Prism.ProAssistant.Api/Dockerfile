FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

ARG NUGET_AUTH_TOKEN

WORKDIR /source

# copy files and restore
COPY . .
RUN dotnet restore

# copy and publish app and libraries
RUN dotnet publish Prism.ProAssistant.Api/Prism.ProAssistant.Api.csproj -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy
EXPOSE 80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apt-get update && apt-get install -y ttf-mscorefonts-installer fontconfig libc6 libc6-dev libgtk2.0-0 libnss3 libatk-bridge2.0-0 libx11-xcb1 libxcb-dri3-0 libdrm-common libgbm1 libasound2 libappindicator3-1 libxrender1 libfontconfig1 libxshmfence1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Prism.ProAssistant.Api.dll"]