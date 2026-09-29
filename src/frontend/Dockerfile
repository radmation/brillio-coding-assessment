FROM node:22-alpine
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

EXPOSE 4200

CMD ["npm", "start"]

# Potential production build strategy.
# # Step 1: Build stage
# FROM node:22-alpine AS build
# WORKDIR /app

# COPY package*.json ./
# RUN npm install

# COPY . ./
# RUN npm run build -- --configuration=production

# # Step 2: Serve stage using Nginx
# FROM nginx:alpine

# # Remove default Nginx website files
# RUN rm -rf /usr/share/nginx/html/*

# # Copy built files directly into html root (Note the trailing dot/slash)
# COPY --from=build /app/dist/frontend/browser/. /usr/share/nginx/html/

# # Copy custom Nginx configuration
# COPY nginx.conf /etc/nginx/conf.d/default.conf

# # Ensure index.html exists
# RUN if [ -f /usr/share/nginx/html/index.csr.html ] && [ ! -f /usr/share/nginx/html/index.html ]; then \
#       cp /usr/share/nginx/html/index.csr.html /usr/share/nginx/html/index.html; \
#     fi

# EXPOSE 80