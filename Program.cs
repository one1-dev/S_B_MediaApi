using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using OpenSearch.Client;
using OpenSearch.Net;
using S_B_MediaApi.Data;
using S_B_MediaApi.Services;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddScoped<ContractSearchService>();


// ---------- OpenSearch client ----------
builder.Services.AddSingleton<IOpenSearchClient>(_ =>
{
    var pool = new SingleNodeConnectionPool(new Uri(cfg["OpenSearch:Uri"]!));
    var settings = new ConnectionSettings(pool)
        .DefaultIndex(cfg["OpenSearch:DefaultIndex"])
        .BasicAuthentication(cfg["OpenSearch:Username"], cfg["OpenSearch:Password"])
        // DEV ONLY: if you're on self-signed TLS in dev
        .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

    return new OpenSearchClient(settings);
});

// ---------- PostgreSQL ----------
// Option A: EF Core
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(cfg.GetConnectionString("Postgres")));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = cfg["Auth:Authority"];   // e.g. https://sso.yourdomain.com
        o.Audience = cfg["Auth:Audience"];    // e.g. customer-media-api
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Contracts.Read", p => p.RequireClaim("scope", "contracts.read"));
    options.AddPolicy("Contracts.Write", p => p.RequireClaim("scope", "contracts.write"));
});

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "S_B_MediaApi",
        Version = "v1",
        Description = "Customer & Media Web Service (generated from controllers)"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter: Bearer {CommandoOne}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // XML comments (only if file exists)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "S_B_MediaApi v1");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
