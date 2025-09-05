using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// --- Configurazione Iniziale ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Configurazione di Firebase ---
var firebaseCredential = GoogleCredential.FromFile("firebase-credentials.json");
FirebaseApp.Create(new AppOptions() { Credential = firebaseCredential });
builder.Services.AddSingleton(new FirestoreDbBuilder
{
    ProjectId = "microfeedlab",
    Credential = firebaseCredential
}.Build());

// --- Configurazione dell'Autenticazione JWT ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/microfeedlab";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/microfeedlab",
            ValidateAudience = true,
            ValidAudience = "microfeedlab",
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
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// --- Definizione dei Modelli ---
public record Post(string Id, string Content, string AuthorId, DateTime CreatedAt);
public record NewPostRequest(string Content);

// --- Definizione degli Endpoint ---
var postsGroup = app.MapGroup("/api/posts");

// Endpoint Protetto: POST /api/posts
postsGroup.MapPost("/", async (NewPostRequest request, ClaimsPrincipal user, FirestoreDb db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // TODO: Implementare la logica per creare un nuovo post.
    // 1. Crea un nuovo oggetto `Post` con un ID univoco (es. Guid.NewGuid().ToString()).
    //    Imposta il contenuto, l'AuthorId (preso dal token) e la data di creazione (DateTime.UtcNow).
    // 2. Ottieni un riferimento alla collezione "posts".
    // 3. Aggiungi il nuovo oggetto `Post` alla collezione.
    // 4. Restituisci Results.Created con l'URL del nuovo post e il post stesso.

    return Results.Ok(new { Message = "Endpoint POST /posts da implementare", UserId = userId, Content = request.Content }); // Placeholder
}).RequireAuthorization();

// Endpoint Pubblico: GET /api/posts/latest
postsGroup.MapGet("/latest", async (FirestoreDb db) =>
{
    // TODO: Implementare la logica per recuperare gli ultimi 20 post.
    // 1. Ottieni un riferimento alla collezione "posts".
    // 2. Crea una query per ordinare i post per "CreatedAt" in ordine decrescente (`OrderByDescending`).
    // 3. Limita i risultati a 20 (`Limit`).
    // 4. Esegui la query per ottenere uno snapshot.
    // 5. Converti i documenti dello snapshot in una lista di oggetti `Post`.
    // 6. Restituisci Results.Ok con la lista.

    return Results.Ok(new List<object>()); // Placeholder
});

app.Run();

