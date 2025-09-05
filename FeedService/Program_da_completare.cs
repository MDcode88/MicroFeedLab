using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
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

// --- Configurazione di Firebase (solo per validazione token) ---
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase-credentials.json")
});

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

// --- Configurazione di HttpClientFactory per la comunicazione tra servizi ---
// Questi indirizzi verranno risolti da Docker Compose.
// Per il testing locale, andranno temporaneamente modificati.
builder.Services.AddHttpClient("PostService", client => { client.BaseAddress = new Uri("http://postservice:8080"); });
builder.Services.AddHttpClient("UserService", client => { client.BaseAddress = new Uri("http://userservice:8080"); });


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
// Questi record devono rispecchiare i modelli degli altri servizi
public record Post(string Id, string Content, string AuthorId, DateTime CreatedAt);
public record UserProfile(string Username, string Bio);

// Questo è il modello che il nostro servizio produrrà
public record FeedItem(string PostId, string Content, string AuthorId, string AuthorUsername);


// --- Definizione degli Endpoint ---

// Endpoint Protetto: GET /api/feed
app.MapGet("/api/feed", static async (ClaimsPrincipal user, IHttpClientFactory httpFactory) =>
{
    // TODO: Implementare la logica di aggregazione del feed.
    // 1. Usa httpFactory per creare un client per "PostService" e uno per "UserService".
    // 2. Chiama l'endpoint GET /api/posts/latest del PostService per ottenere gli ultimi post.
    //    (Suggerimento: usa `GetFromJsonAsync<List<Post>>`).
    //    Gestisci il caso in cui non ci siano post.
    // 3. Dalla lista di post, estrai tutti gli ID degli autori (`AuthorId`) in una lista, assicurandoti che siano unici.
    //    (Suggerimento: `posts.Select(...).Distinct()`).
    // 4. Per ogni ID autore unico, chiama l'endpoint GET /api/users/{userId} del UserService per ottenere il profilo.
    //    Salva i profili recuperati in un `Dictionary<string, UserProfile>` per un accesso efficiente.
            }).RequireAuthorization();

            app.Run();
        }
    }
}

    return Results.Ok(new List<object>()); // Placeholder

}).RequireAuthorization();

app.Run();

