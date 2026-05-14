# ==========================================
# Stage 1: Build do Vue
# ==========================================
FROM node:24-bookworm-slim AS builder

# Definindo o diretório de trabalho
WORKDIR /app

# Copiar arquivos relacionados a pacotes para o cache do docker
COPY package.json package-lock.json ./

# Instalar depeências através do NPM (utilizando npm ci (clean install) pois ele segue estritamente o package-lock) - também utilizamos cache para builds futuros mais rápidos
RUN --mount=type=cache,target=/root/.npm npm ci

# Copiar o resto do código da aplicação frontend
COPY . .

# Buildar a aplicação Vue
RUN npm run build

# =======================
# Stage 2: Preparar o NGINX
# =======================
FROM dhi.io/nginx:1.30-debian13-dev AS runner

# Copiar o arquivo de config do nginx
COPY nginx.conf /etc/nginx/nginx.conf

# Criar o usuário e grupo nginx para ele poder ler os arquivos e jogar o resultado do build anterior dentro da pasta padrão do nginx
COPY --chown=nginx:nginx --from=builder /app/dist /usr/share/nginx/html

# Definimos o usuário diferente do root, assim ele não tem acesso a portas privilegiadas (abaixo de 1024)
USER nginx

# Expondo a porta padrão para o nginx
EXPOSE 8080

# Iniciar o NGINX diretamente
ENTRYPOINT ["nginx", "-c", "/etc/nginx/nginx.conf"]
CMD ["-g", "daemon off;"]
