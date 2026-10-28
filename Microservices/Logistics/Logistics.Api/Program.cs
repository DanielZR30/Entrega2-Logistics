using Logistics.Api.Configuration;
using Logistics.Application;
using Logistics.Persistence;
using Logistics.Persistence.Seeds;

DotEnvLoader.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

WebApplication app = builder.Build();

try
{
    await DataBaseSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "No se pudo ejecutar el seeder en el arranque.");
}

app.Run();
