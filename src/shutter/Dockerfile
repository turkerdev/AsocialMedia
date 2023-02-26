FROM node:18-alpine AS deps
WORKDIR /app
COPY src/shutter/package*.json .
RUN npm install

FROM node:18-alpine AS production-deps
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY src/shutter/package*.json .
RUN npm prune --omit=dev

FROM node:18-alpine AS protobuf
WORKDIR /app
RUN apk update
RUN apk add protoc
RUN npm i -g ts-proto
COPY pb/*.proto .
RUN mkdir dist
RUN protoc --ts_proto_opt=esModuleInterop=true --ts_proto_opt=outputServices=grpc-js --ts_proto_out=dist *.proto

FROM node:18-alpine AS build
WORKDIR /app
COPY --from=deps /app/node_modules /app/node_modules
COPY src/shutter .
COPY --from=protobuf /app/dist src/protos
RUN npm run build

FROM ubuntu:20.04 AS browser
WORKDIR /app
RUN apt-get update && apt-get install -y curl
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && apt-get install -y nodejs
RUN npm i -g playwright
RUN playwright install chromium

FROM node:18
WORKDIR /app
COPY --from=production-deps /app/node_modules ./node_modules
RUN npx playwright install-deps
COPY --from=browser /root/.cache/ms-playwright /root/.cache/ms-playwright
COPY --from=build /app/dist .
ENTRYPOINT [ "node", "index.js" ]