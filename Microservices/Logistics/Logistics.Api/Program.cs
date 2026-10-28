using Logistics.Api.Configuration;
using Logistics.Api.Middleware;
using Logistics.Application;
using Logistics.Persistence;
using Logistics.Persistence.Seeds;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Reflection;

DotEnvLoader.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SGCAE API - Control de Acceso de Empleados (Seguimiento 2)",
        Version = "v1",
        Description = "API desarrollada bajo Clean Architecture, DDD, CQRS y EF Core para gestionar empleados, credenciales, puntos de acceso y el registro de entradas y salidas de una empresa logística."
    });

    // Documentación XML de los controladores en Swagger y Scalar
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

WebApplication app = builder.Build();

// Manejo global de excepciones de negocio y errores no controlados
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Swagger UI clásico
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGCAE API v1");
        c.RoutePrefix = "swagger";
    });

    // Interfaz moderna y estilizada: Scalar API Reference
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Control de Acceso SGCAE - Seguimiento 2")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    // Redireccionar raíz al dashboard moderno
    app.MapGet("/", () => Results.Redirect("/scalar/v1"));
}

try
{
    await DataBaseSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "No se pudo ejecutar el seeder en el arranque.");
}

// Comprobación de estado del servicio
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

