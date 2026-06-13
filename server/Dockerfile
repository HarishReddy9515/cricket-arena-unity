FROM node:20-alpine

WORKDIR /app
COPY package.json ./
COPY authoritative-server.js ./

ENV PORT=8790
EXPOSE 8790

CMD ["node", "authoritative-server.js"]
