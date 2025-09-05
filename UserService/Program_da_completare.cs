using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// --- Configurazione Iniziale ---

// Aggiunge i servizi al container DI (Dependency Injection).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura CORS per permettere al frontend di comunicare con questo servizio.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:8080") // L'indirizzo del frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Configurazione di Firebase ---

// Inizializza l'SDK di Firebase Admin usando le credenziali.
var firebaseCredential = GoogleCredential.FromFile("firebase-credentials.json");
FirebaseApp.Create(new AppOptions() { Credential = firebaseCredential });

// Crea un'istanza del client Firestore e la rende disponibile tramite DI.
builder.Services.AddSingleton(new FirestoreDbBuilder
{
    ProjectId = "microfeedlab", // Sostituisci con il tuo Project ID di Firebase
    Credential = firebaseCredential
}.Build());

// --- Configurazione dell'Autenticazione JWT ---

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/microfeedlab"; // Sostituisci con il tuo Project ID
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/microfeedlab", // Sostituisci con il tuo Project ID
            ValidateAudience = true,
            ValidAudience = "microfeedlab", // Sostituisci con il tuo Project ID
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(); // Applica la policy CORS
app.UseAuthentication(); // Prima Autenticazione...
app.UseAuthorization();  // ...poi Autorizzazione

// --- Definizione dei Modelli ---

public record UserProfile(string Username, string Bio);

// --- Definizione degli Endpoint ---

var usersGroup = app.MapGroup("/api/users");

// Endpoint Pubblico: GET /api/users/{userId}
usersGroup.MapGet("/{userId}", async (string userId, FirestoreDb db) =>
{
    // TODO: Implementare la logica per recuperare un profilo utente dal database.
    // 1. Crea un riferimento al documento dell'utente nella collezione "users".
    // 2. Recupera uno snapshot del documento.
    // 3. Se lo snapshot non esiste, restituisci Results.NotFound().
    // 4. Se esiste, convertilo in un oggetto UserProfile e restituisci Results.Ok().

    return Results.NoContent(); // Placeholder
});

// Endpoint Protetto: GET /api/users/me
usersGroup.MapGet("/me", async (ClaimsPrincipal user, FirestoreDb db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    // TODO: Implementare la logica per recuperare il PROPRIO profilo utente.
    // La logica è identica all'endpoint precedente, ma usa lo `userId` preso dal token.

    return Results.Ok(new { Message = "Endpoint /me da implementare", UserId = userId }); // Placeholder
}).RequireAuthorization();

// Endpoint Protetto: POST /api/users/me
usersGroup.MapPost("/me", async (UserProfile profile, ClaimsPrincipal user, FirestoreDb db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    // TODO: Implementare la logica per creare/aggiornare il profilo.
    // 1. Crea un riferimento al documento dell'utente.
    // 2. Usa il metodo `SetAsync` con `SetOptions.MergeAll` per scrivere i dati.
    // 3. Restituisci Results.Ok().

    return Results.Ok(new { Message = "Endpoint POST /me da implementare", UserId = userId, ReceivedProfile = profile }); // Placeholder
}).RequireAuthorization();

app.Run();

