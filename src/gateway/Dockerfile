FROM node:18-alpine AS deps
WORKDIR /app
COPY src/gateway/package*.json .
RUN npm install

FROM node:18-alpine AS production-deps
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY src/gateway/package*.json .
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
COPY src/gateway .
COPY --from=protobuf /app/dist src/protos
RUN npx prisma generate
RUN npm run typecheck
RUN npm run build

FROM node:18-alpine
WORKDIR /app
COPY --from=production-deps /app/node_modules ./node_modules
COPY --from=build /app/node_modules/.prisma /app/node_modules/.prisma
COPY --from=build /app/prisma /app/prisma
COPY --from=build /app/dist .
ENTRYPOINT [ "node", "main.js" ]