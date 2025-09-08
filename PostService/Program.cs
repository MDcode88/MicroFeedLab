using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

// --- Configurazione Iniziale e Pipeline (Istruzioni di primo livello) ---
var builder = WebApplication.CreateBuilder(args);
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

// Configurazione di Firebase
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "firebase-credentials.json"))
});
// Configurazione dell'Autenticazione JWT
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
builder.Services.AddSingleton(provider =>
    FirestoreDb.Create("microfeedlab")
);

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// --- Definizione degli Endpoint ---
var postsGroup = app.MapGroup("/api/posts");

// Endpoint Protetto: POST /api/posts
postsGroup.MapPost("/", async (NewPostRequest request, ClaimsPrincipal user, FirestoreDb db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

    var post = new Post
    {
        Id = "",
        AuthorId = userId,
        Content = request.Content,
        Timestamp = Timestamp.GetCurrentTimestamp()
    };

    var docRef = await db.Collection("posts").AddAsync(post);
    var createdPost = post with { Id = docRef.Id };

    return Results.Created($"/api/posts/{createdPost.Id}", createdPost);
}).RequireAuthorization();

// Endpoint Pubblico: GET /api/posts/user/{userId}
postsGroup.MapGet("/user/{userId}", async (string userId, FirestoreDb db) =>
{
    var query = db.Collection("posts")
                    .WhereEqualTo("AuthorId", userId)
                    .OrderByDescending("Timestamp");

    var snapshot = await query.GetSnapshotAsync();
    var posts = snapshot.Documents.Select(doc =>
    {
        var post = doc.ConvertTo<Post>();
        return post with { Id = doc.Id };
    }).ToList();

    return Results.Ok(posts);
});

// Endpoint Pubblico: GET /api/posts/latest
postsGroup.MapGet("/latest", async (FirestoreDb db) =>
{
    var query = db.Collection("posts")
                    .OrderByDescending("Timestamp")
                    .Limit(20);

    var snapshot = await query.GetSnapshotAsync();
    var posts = snapshot.Documents.Select(doc =>
    {
        var post = doc.ConvertTo<Post>();
        return post with { Id = doc.Id };
    }).ToList();

    return Results.Ok(posts);
});

app.Run();

// --- Definizione dei Modelli (dichiarazioni di tipo) ---
public record Post
{
    public required string Id { get; init; }
    public required string Content { get; init; }
    public required string AuthorId { get; init; }
    public required Timestamp Timestamp { get; init; }
}
public record NewPostRequest(string Content);