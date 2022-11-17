FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG NUGET_AUTH_TOKEN

WORKDIR /source

# copy files and restore
COPY . .
RUN dotnet restore

# copy and publish app and libraries
RUN dotnet publish Prism.ProAssistant.Api/Prism.ProAssistant.Api.csproj -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
EXPOSE 80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Prism.ProAssistant.Api.dll"]