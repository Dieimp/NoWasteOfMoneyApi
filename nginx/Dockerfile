FROM nginx:alpine

# Instala openssl para gerar o certificado autoassinado
RUN apk add --no-cache openssl

# Cria o diretório para os certificados SSL
RUN mkdir -p /etc/nginx/ssl

# Gera a chave e o certificado autoassinado salvando dentro do container
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /etc/nginx/ssl/nginx.key \
    -out /etc/nginx/ssl/nginx.crt \
    -subj "/C=BR/ST=SP/L=SaoPaulo/O=NoWasteOfMoney/CN=nowasteofmoney.local" \
    -addext "subjectAltName=DNS:nowasteofmoney.local,DNS:localhost,IP:127.0.0.1"

# Copia a configuração customizada do Nginx
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80 443
