# Fase 1: Build
# Usiamo l'immagine ufficiale dell'SDK di .NET 8.0 come base per compilare il progetto.
# L'SDK contiene tutti gli strumenti necessari per la compilazione.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia i file di progetto (.csproj) e ripristina le dipendenze (NuGet packages).
# Copiare solo il .csproj prima e fare il restore è un trucco di caching:
# se i pacchetti non cambiano, Docker non rieseguirà questo strato.
COPY *.csproj ./
RUN dotnet restore

# Copia tutto il resto del codice sorgente.
COPY . ./

# Pubblica l'applicazione in modalità Release, ottimizzata per l'esecuzione.
# L'output verrà messo nella cartella /app/out.
RUN dotnet publish -c Release -o out

# Fase 2: Runtime
# Usiamo un'immagine molto più leggera che contiene solo il runtime di ASP.NET Core.
# Questo rende l'immagine finale molto più piccola e sicura.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia solo l'output della compilazione dalla fase di 'build'.
COPY --from=build /app/out .

# Specifica il comando che verrà eseguito all'avvio del container.
# In questo caso, avvia l'applicazione .NET.
ENTRYPOINT ["dotnet", "UserService.dll"]
