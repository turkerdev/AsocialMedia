FROM node:18-alpine AS deps
WORKDIR /app
COPY src/uploadservice/package*.json .
RUN npm install

FROM node:18-alpine AS production-deps
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY src/uploadservice/package*.json .
RUN npm prune --omit=dev

FROM node:18-alpine AS build
WORKDIR /app
COPY --from=deps /app/node_modules /app/node_modules
COPY src/uploadservice .
RUN npm run build

FROM node:18-alpine
WORKDIR /app
COPY --from=production-deps /app/node_modules ./node_modules
COPY --from=build /app/dist .