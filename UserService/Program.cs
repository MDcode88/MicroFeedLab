using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Google.Cloud.Firestore;

// --- CONFIGURAZIONE (GIÀ PRONTA) ---
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var firebaseCredentialPath = builder.Configuration["Firebase:CredentialPath"]!;
var projectId = builder.Configuration["Firebase:ProjectId"]!;
FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile(firebaseCredentialPath) });
builder.Services.AddSingleton(FirestoreDb.Create(projectId));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();
[FirestoreData]
public record UserProfile([property: FirestoreProperty] string Username, [property: FirestoreProperty] string Bio);

// --- ENDPOINT DA IMPLEMENTARE ---
var usersApi = app.MapGroup("/api/users").RequireAuthorization();

// Endpoint pubblico per ottenere un profilo utente
app.MapGet("/api/users/{userId}", async (string userId, FirestoreDb db) =>
{

    // TODO: Implementare la logica per recuperare un profilo utente
    // 1. Ottenere il riferimento al documento: db.Collection("users").Document(userId)
    // 2. Eseguire la chiamata asincrona: await docRef.GetSnapshotAsync()
    // 3. Controllare se lo snapshot esiste (snapshot.Exists)
    // 4. Se esiste, convertirlo in UserProfile e restituire Results.Ok(profile)
    // 5. Altrimenti, restituire Results.NotFound()

    return Results.Ok($"TODO: Implementare GET /api/users/{userId}");

}).AllowAnonymous();

// Endpoint protetto per ottenere il proprio profilo
usersApi.MapGet("/me", async (HttpContext ctx, FirestoreDb db) =>
{
    var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Results.Unauthorized();

    // TODO: Implementare la logica per recuperare il PROPRIO profilo
    // La logica è identica all'endpoint precedente, ma si usa lo `userId` preso dal token.

    return Results.Ok("TODO: Implementare GET /api/users/me");
});

// Endpoint protetto per creare/aggiornare il proprio profilo
usersApi.MapPost("/me", async (UserProfile profile, HttpContext ctx, FirestoreDb db) =>
{
    var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Results.Unauthorized();

    // TODO: Implementare la logica per salvare il profilo
    // 1. Ottenere il riferimento al documento: db.Collection("users").Document(userId)
    // 2. Salvare l'oggetto `profile`: await docRef.SetAsync(profile, SetOptions.MergeAll)
    // 3. Restituire Results.Ok()

    return Results.Ok("TODO: Implementare POST /api/users/me");
});

app.Run();
