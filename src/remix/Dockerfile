FROM node:18-alpine
WORKDIR /app
COPY package*.json .
RUN npm install --ignore-scripts
COPY . .
CMD npm run postinstall && npm run dev