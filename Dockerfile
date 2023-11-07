FROM node:18-alpine AS typescript
WORKDIR /app
RUN apk update
RUN apk add protoc
RUN npm i -g ts-proto
COPY pb/*.proto .
RUN mkdir dist
RUN protoc --ts_proto_opt=esModuleInterop=true --ts_proto_opt=outputServices=grpc-js --ts_proto_out=dist *.proto