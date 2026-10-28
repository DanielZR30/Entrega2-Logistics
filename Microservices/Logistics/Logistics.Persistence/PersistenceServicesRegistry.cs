using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Persistence.Repositories;
using Logistics.Persistence.Seeds;
using Logistics.Persistence.Seeds.AccessPoints;
using Logistics.Persistence.Seeds.AccessRecords;
using Logistics.Persistence.Seeds.Credentials;
using Logistics.Persistence.Seeds.Employees;
using Logistics.Persistence.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logistics.Persistence
{
    public static class PersistenceServicesRegistry
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("MyConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString = configuration["DB_CONNECTION_STRING"]
                        ?? Environment.GetEnvironmentVariable("ConnectionStrings__MyConnection")
                        ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
                }

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    var server = configuration["DB_SERVER"] ?? Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost,1433";
                    var database = configuration["DB_NAME"] ?? Environment.GetEnvironmentVariable("DB_NAME") ?? "LogisticsDb";
                    var user = configuration["DB_USER"] ?? Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
                    var password = configuration["MSSQL_SA_PASSWORD"]
                        ?? configuration["DB_PASSWORD"]
                        ?? Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")
                        ?? Environment.GetEnvironmentVariable("DB_PASSWORD");

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        connectionString = $"Server={server};Database={database};User Id={user};Password={password};TrustServerCertificate=True;MultipleActiveResultSets=True";
                    }
                }

                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<ICredentialsRepository, CredentialsRepository>();
            services.AddScoped<IAccessPointsRepository, AccessPointsRepository>();
            services.AddScoped<IAccessRecordsRepository, AccessRecordsRepository>();

            // Seeders
            services.AddScoped<IDataSeeder, AccessPointSeeder>();
            services.AddScoped<IDataSeeder, EmployeeSeeder>();
            services.AddScoped<IDataSeeder, CredentialSeeder>();
            services.AddScoped<IDataSeeder, AccessRecordSeeder>();

            return services;
        }
    }
}
