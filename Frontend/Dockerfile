# Fase 1: Build dell'app Blazor
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Fase 2: Server Nginx per servire i file statici

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
# Rimuovi il contenuto di default
RUN rm -rf ./*
# Copia i file pubblicati dalla fase di build
COPY --from=build /app/publish/wwwroot .
# Copia la configurazione di Nginx per supportare le SPA
COPY nginx.conf /etc/nginx/conf.d/default.conf
