using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenSearch.Client;
using OpenSearch.Net;
using S_B_MicroService.Data;
using S_B_MicroService.Domain.Services;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

// Controllers
builder.Services.AddControllers();

// EF Core (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(cfg.GetConnectionString("Postgres")));

// OpenSearch client
builder.Services.AddSingleton<IOpenSearchClient>(_ =>
{
    var pool = new SingleNodeConnectionPool(new Uri(cfg["OpenSearch:Uri"]!));
    var settings = new ConnectionSettings(pool)
        .DefaultIndex(cfg["OpenSearch:DefaultIndex"])
        .BasicAuthentication(cfg["OpenSearch:Username"], cfg["OpenSearch:Password"])
        // DEV ONLY: allow self-signed in local envs
        .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

    return new OpenSearchClient(settings);
});

// Domain services
builder.Services.AddScoped<ContractSearchService>();

// AuthN/Z (SSO/JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = cfg["Auth:Authority"];   // e.g. https://sso.domain.com
        o.Audience = cfg["Auth:Audience"];    // e.g. S_B_MediaApi
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "S_B_MicroService",
        Version = "v1",
        Description = "Customer & Media Web Service (generated from controllers)"
    });

    // Avoid schema/route collisions
    c.CustomSchemaIds(t => t.FullName!.Replace('+', '.'));
    c.ResolveConflictingActions(apiDescs => apiDescs.First());
    c.SupportNonNullableReferenceTypes();

    // Bearer auth for Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter: Bearer {your_access_token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme
            { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
          Array.Empty<string>() }
    });

    // XML comments (if you enabled <GenerateDocumentationFile/> in .csproj)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

// Swagger UI (enabled in Development by default)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "S_B_MediaApi v1"));
}
else
{
    // uncomment if you want Swagger in non-dev too:
    // app.UseSwagger();
    // app.UseSwaggerUI(c =>
    //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "S_B_MediaApi v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
