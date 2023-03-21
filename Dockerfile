# Build the web app
FROM node:18-alpine AS web-deps

WORKDIR /app
COPY ./web/package.json ./web/yarn.lock ./
COPY ./web/.yarnrc.docker.yml .yarnrc.yml

RUN corepack enable \
    && corepack prepare yarn@stable --activate \
    && yarn set version 3
    
RUN yarn install --immutable

FROM node:18-alpine AS web-builder

WORKDIR /app

ENV NODE_ENV=production

COPY ./web .

COPY --from=web-deps /app/.pnp.cjs ./
COPY --from=web-deps /app/.pnp.loader.mjs ./
COPY --from=web-deps /app/.yarn ./.yarn
COPY --from=web-deps /app/.yarnrc.yml ./.yarnrc.yml

RUN corepack enable \
    && corepack prepare yarn@stable --activate \
    && yarn set version 3
    
RUN yarn install --immutable
RUN yarn export

# Build the API
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS net-builder

WORKDIR /source

COPY ./api .
RUN dotnet restore
RUN dotnet publish Prism.ProAssistant.Api/Prism.ProAssistant.Api.csproj -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy
EXPOSE 80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apt-get update && apt-get install -y ttf-mscorefonts-installer fontconfig libc6 libc6-dev libgtk2.0-0 libnss3 libatk-bridge2.0-0 libx11-xcb1 libxcb-dri3-0 libdrm-common libgbm1 libasound2 libappindicator3-1 libxrender1 libfontconfig1 libxshmfence1
WORKDIR /app
COPY --from=net-builder /app .
COPY --from=web-builder /app/out ./wwwroot
ENTRYPOINT ["dotnet", "Prism.ProAssistant.Api.dll"]