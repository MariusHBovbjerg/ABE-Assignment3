FROM node:lts-alpine AS base
RUN apk update
WORKDIR /app
ENV YARN_CACHE_FOLDER=.yarn-cache

FROM base AS builder
COPY ./package.json ./package.json
COPY ./yarn.lock ./yarn.lock
RUN yarn install --cache-folder ${YARN_CACHE_FOLDER}
COPY ./src ./src
RUN yarn build


FROM base AS runner
COPY ./dist/ ./dist/
COPY ./node_modules ./node_modules/
CMD node ./dist/main
EXPOSE 3001