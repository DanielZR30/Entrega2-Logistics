using Logistics.Api.Configuration;
using Logistics.Api.Middleware;
using Logistics.Application;
using Logistics.Persistence;
using Logistics.Persistence.Seeds;
using Microsoft.OpenApi;
using System.Reflection;

DotEnvLoader.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Gestión de Empleados (Seguimiento 2)",
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
    // Swagger UI: documentación interactiva de la API
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestión de Empleados API v1");
        c.RoutePrefix = "swagger";
    });

    // Redireccionar raíz a la documentación interactiva
    app.MapGet("/", () => Results.Redirect("/swagger"));
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

