FROM node:18 AS development
WORKDIR /app
COPY package*.json .
COPY node_modules .
RUN PLAYWRIGHT_SKIP_BROWSER_DOWNLOAD=1 npm install
RUN npx playwright install --with-deps chromium