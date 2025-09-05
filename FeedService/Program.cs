using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

// --- CONFIGURAZIONE (GIÀ PRONTA) ---
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var firebaseCredentialPath = builder.Configuration["Firebase:CredentialPath"]!;
var projectId = builder.Configuration["Firebase:ProjectId"]!;
FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile(firebaseCredentialPath) });
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
builder.Services.AddHttpClient("UserService", client => client.BaseAddress = new Uri(builder.Configuration["Services:UserUrl"]!));
builder.Services.AddHttpClient("PostService", client => client.BaseAddress = new Uri(builder.Configuration["Services:PostUrl"]!));
var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();
public record Post([property: JsonPropertyName("id")] string Id, [property: JsonPropertyName("authorId")] string AuthorId, [property: JsonPropertyName("content")] string Content);
public record UserProfile([property: JsonPropertyName("username")] string Username);
public record FeedItem(string PostId, string Content, string AuthorId, string AuthorUsername);

// --- ENDPOINT DA IMPLEMENTARE ---
app.MapGet("/api/feed", async (IHttpClientFactory clientFactory) =>
{
    var postClient = clientFactory.CreateClient("PostService");
    var userClient = clientFactory.CreateClient("UserService");

    // TODO: Implementare la logica di aggregazione
    // 1. Chiamare PostService per ottenere gli ultimi post
    //    `var posts = await postClient.GetFromJsonAsync<List<Post>>("/api/posts/latest");`
    //    Gestire il caso in cui `posts` sia nullo o vuoto.

    // 2. Per ogni post, chiamare UserService per ottenere il profilo dell'autore.
    //    Usare un approccio parallelo per efficienza con `Task.WhenAll`.
    //    `var feedTasks = posts.Select(async post => { ... });`

    // 3. Dentro il .Select:
    //    a. Chiamare `await userClient.GetFromJsonAsync<UserProfile>($"/api/users/{post.AuthorId}")`
    //    b. Usare un try-catch per gestire il caso in cui un utente non esista più.
    //    c. Creare e restituire un nuovo `FeedItem` con i dati combinati.

    // 4. Eseguire tutti i task: `var feedItems = await Task.WhenAll(feedTasks);`

    // 5. Restituire `Results.Ok(feedItems)`.

    return Results.Ok("TODO: Implementare GET /api/feed");

}).RequireAuthorization();

app.Run();
