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
public record NewPostRequest(string Content);
[FirestoreData]
public record Post
{
    [FirestoreDocumentId] public string? Id { get; init; }
    [FirestoreProperty] public string AuthorId { get; init; } = "";
    [FirestoreProperty] public string Content { get; init; } = "";
    [FirestoreProperty] public Timestamp Timestamp { get; init; }
}

// --- ENDPOINT DA IMPLEMENTARE ---
var postsApi = app.MapGroup("/api/posts");

postsApi.MapPost("/", async (NewPostRequest req, HttpContext ctx, FirestoreDb db) =>
{
    var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Results.Unauthorized();

    // TODO: Implementare la logica per creare un nuovo post
    // 1. Creare un'istanza di `Post` con i dati necessari (AuthorId, Content, Timestamp).
    //    Suggerimento: per il timestamp, usare `Timestamp.GetCurrentTimestamp()`.
    // 2. Aggiungere il nuovo oggetto alla collezione "posts": `await db.Collection("posts").AddAsync(post)`
    // 3. Restituire `Results.Created()` con il post creato.

    return Results.Ok("TODO: Implementare POST /api/posts");

}).RequireAuthorization();

postsApi.MapGet("/user/{userId}", async (string userId, FirestoreDb db) =>
{

    // TODO: Implementare la logica per recuperare i post di un utente
    // 1. Creare una query: `db.Collection("posts").WhereEqualTo("AuthorId", userId).OrderByDescending("Timestamp")`
    // 2. Eseguire la query: `await query.GetSnapshotAsync()`
    // 3. Iterare sui `snapshot.Documents`, convertirli in oggetti `Post` e aggiungerli a una lista.
    // 4. Restituire `Results.Ok(listaDiPost)`.

    return Results.Ok($"TODO: Implementare GET /api/posts/user/{userId}");
});

postsApi.MapGet("/latest", async (FirestoreDb db) =>
{

    // TODO: Implementare la logica per recuperare gli ultimi 20 post
    // 1. Creare una query: `db.Collection("posts").OrderByDescending("Timestamp").Limit(20)`
    // 2. Eseguire la query e processare i risultati come nel punto precedente.
    // 3. Restituire `Results.Ok(listaDiPost)`.

    return Results.Ok("TODO: Implementare GET /api/posts/latest");
});

app.Run();
