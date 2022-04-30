FROM node:lts-alpine AS base
RUN apk update
WORKDIR /app
ENV YARN_CACHE_FOLDER=.yarn-cache

FROM base AS builder
WORKDIR /build
COPY ./package.json ./package.json
COPY ./yarn.lock ./yarn.lock
RUN yarn install --cache-folder ${YARN_CACHE_FOLDER}
COPY . .
RUN yarn build


FROM base AS runner
WORKDIR /app
COPY --from=builder /build/dist ./dist
COPY --from=builder /build/node_modules node_modules
CMD node ./dist/main
EXPOSE 3001