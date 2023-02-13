# Rebuild the source code
FROM node:18-alpine AS deps
WORKDIR /app
COPY .pnp.cjs .yarnrc.yml package.json yarn.lock* ./

# Install Yarn
RUN corepack enable
RUN corepack prepare yarn@stable --activate

RUN yarn install --immutable --inline-builds


# Production image, copy all the files and run next
FROM node:18-alpine AS runner
WORKDIR /app

RUN corepack enable
RUN corepack prepare yarn@stable --activate

COPY --from=deps /app/.yarn ./.yarn
COPY --from=deps /app/.pnp.cjs ./pnp.cjs
COPY . .
RUN yarn build

ENV NODE_ENV production

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

COPY entrypoint.sh .

RUN apk add --no-cache --upgrade bash
RUN apk add dos2unix --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/community/ --allow-untrusted

RUN chmod +x ./entrypoint.sh
RUN dos2unix ./entrypoint.sh

RUN chown -R nextjs:nodejs /app

USER nextjs

EXPOSE 3000

ENV PORT 3000

ENTRYPOINT ["/app/entrypoint.sh"]
CMD ["yarn","node", ".next/standalone/server.js"]