# Fase 1: Build
# Usiamo l'immagine ufficiale dell'SDK di .NET 8.0 come base per compilare il progetto.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia i file di progetto (.csproj) e ripristina le dipendenze.
COPY *.csproj ./
RUN dotnet restore

# Copia tutto il resto del codice sorgente.
COPY . ./

# Pubblica l'applicazione in modalità Release.
RUN dotnet publish -c Release -o out

# Fase 2: Runtime
# Usiamo l'immagine leggera del runtime di ASP.NET Core.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia solo l'output della compilazione dalla fase di 'build'.
COPY --from=build /app/out .

# Specifica il comando che avvia l'applicazione.
ENTRYPOINT ["dotnet", "PostService.dll"]
