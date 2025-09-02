// using Microsoft.OpenApi.Models;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "S_B_MediaApi", Version = "v1" });
// });

// var app = builder.Build();

// // *only* in dev
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "S_B_MediaApi v1");
//     });
// }


// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();
// app.Run();


using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "S_B_MediaApi",
        Version = "v1",
        Description = "Customer & Media Web Service (generated from controllers)"
    });

    // XML comments for nice descriptions in Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

// Swagger UI at /swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "S_B_MediaApi v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
