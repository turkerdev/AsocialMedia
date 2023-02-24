FROM node:18-alpine AS build
WORKDIR /app
COPY src/shutter/package*.json .
RUN npm install
COPY src/shutter .
RUN npm run build

FROM node:18-alpine
WORKDIR /app
COPY --from=build /app/package.json .
RUN npm install --production
COPY --from=build /app/dist .
ENTRYPOINT [ "node", "index.js" ]