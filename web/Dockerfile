FROM node:18-alpine AS deps

WORKDIR /app
COPY package.json yarn.lock ./
COPY .yarnrc.docker.yml .yarnrc.yml

RUN corepack enable \
    && corepack prepare yarn@stable --activate \
    && yarn set version 3
    
RUN yarn install --immutable

FROM node:18-alpine AS builder

WORKDIR /app

COPY . .

COPY --from=deps /app/.pnp.cjs ./
COPY --from=deps /app/.pnp.loader.mjs ./
COPY --from=deps /app/.yarn ./.yarn
COPY --from=deps /app/.yarnrc.yml ./.yarnrc.yml

RUN corepack enable \
    && corepack prepare yarn@stable --activate \
    && yarn set version 3
    
RUN yarn install --immutable
RUN yarn build

FROM node:18-alpine

ENV NODE_ENV production

RUN addgroup -g 1001 -S nodejs
RUN adduser -S nextjs -u 1001

RUN corepack enable \
    && corepack prepare yarn@stable --activate \
    && yarn set version 3

RUN rm -rf /app
WORKDIR /app

COPY --from=builder --chown=nextjs:nodejs /app/package.json /app/yarn.lock ./
COPY --from=builder --chown=nextjs:nodejs /app/.pnp.cjs ./
COPY --from=builder --chown=nextjs:nodejs /app/.pnp.loader.mjs ./
COPY --from=builder --chown=nextjs:nodejs /app/.yarn ./.yarn
COPY --from=builder --chown=nextjs:nodejs /app/.yarnrc.yml ./.yarnrc.yml

COPY --from=builder --chown=nextjs:nodejs /app/.next/ ./.next
COPY --from=builder --chown=nextjs:nodejs /app/public ./public

RUN yarn install --immutable

USER nextjs

EXPOSE 3000

CMD ["yarn", "start"]
