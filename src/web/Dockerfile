# Rebuild the source code only when needed
FROM node:18-alpine AS builder
WORKDIR /app

COPY . .
COPY .yarnrc.docker.yml .yarnrc.yml

RUN yarn
RUN yarn build

# Production image, copy all the files and run next
FROM node:18-alpine AS runner
WORKDIR /app

ENV NODE_ENV production

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/public ./public
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static

COPY entrypoint.sh .

RUN apk add --no-cache --upgrade bash
RUN apk add dos2unix --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/community/ --allow-untrusted

RUN chmod +x ./entrypoint.sh
RUN dos2unix ./entrypoint.sh

USER nextjs

EXPOSE 3000

ENV PORT 3000

ENTRYPOINT ["/app/entrypoint.sh"]
CMD ["node", "server.js"]