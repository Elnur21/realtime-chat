using RealTimeChat.Settings;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MediatR;
using System.Reflection;

using RealTimeChat.Extensions;
using RealTimeChat.Middlewares;
using RealTimeChat.Hubs;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetSection("Issuer").Value ?? "RealTimeChatIssuer",
            ValidAudience = builder.Configuration.GetSection("Audience").Value ?? "RealTimeChatAudience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Key").Value ?? "MySecretKey123456789012345678901234567890")
            )
        };

        // Don't require authentication by default
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        // Configure JWT for SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
if (mongoDbSettings == null || string.IsNullOrEmpty(mongoDbSettings.ConnectionString))
{
    throw new InvalidOperationException("MongoDbSettings or ConnectionString is not configured properly.");
}
var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

builder.Services.AddSingleton<IMongoClient>(mongoClient);

builder.Services.AddMongoCollections(mongoClient, mongoDbSettings.DatabaseName);

// Add Gemini AI Service
builder.Services.AddGeminiAiService(builder.Configuration);

builder.Services.AddControllers();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // required for SignalR
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RealTimeChat", Version = "v1" });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.MapHub<ChatHub>("/chatHub");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
