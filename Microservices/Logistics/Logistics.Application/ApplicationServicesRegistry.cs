using Logistics.Application.UseCases.AccessPoints.Commands.CreateAccessPoint;
using Logistics.Application.UseCases.AccessPoints.Commands.UpdateAccessPointStatus;
using Logistics.Application.UseCases.AccessPoints.Queries.GetAccessPointsList;
using Logistics.Application.UseCases.AccessRecords.Commands.RegisterAccess;
using Logistics.Application.UseCases.AccessRecords.Queries.GetAccessHistory;
using Logistics.Application.UseCases.Credentials.Commands.AssignCredential;
using Logistics.Application.UseCases.Credentials.Commands.BlockCredential;
using Logistics.Application.UseCases.Credentials.Commands.RevokeCredential;
using Logistics.Application.UseCases.Credentials.Queries.GetCredentialByCode;
using Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee;
using Logistics.Application.UseCases.Employees.Commands.CreateEmployee;
using Logistics.Application.UseCases.Employees.Commands.DeactivateEmployee;
using Logistics.Application.UseCases.Employees.Commands.UpdateEmployee;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeeById;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeesList;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Microsoft.Extensions.DependencyInjection;

namespace Logistics.Application
{
    public static class ApplicationServicesRegistry
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Mediator
            services.AddScoped<IMediator, SimpleMediator>();

            // Use Cases - Queries
            services.AddScoped<IRequestHandler<GetEmployeesListQuery, PaginationResponse<EmployeeListItemDTO>>, GetEmployeesListUseCase>();
            services.AddScoped<IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailsDTO?>, GetEmployeeByIdUseCase>();
            services.AddScoped<IRequestHandler<GetCredentialsByEmployeeQuery, PaginationResponse<CredentialDTO>>, GetCredentialsByEmployeeUseCase>();
            services.AddScoped<IRequestHandler<GetCredentialByCodeQuery, CredentialDTO?>, GetCredentialByCodeUseCase>();
            services.AddScoped<IRequestHandler<GetAccessHistoryQuery, PaginationResponse<AccessRecordDTO>>, GetAccessHistoryUseCase>();
            services.AddScoped<IRequestHandler<GetAccessPointsListQuery, PaginationResponse<AccessPointDTO>>, GetAccessPointsListUseCase>();

            // Use Cases - Commands
            services.AddScoped<IRequestHandler<CreateEmployeeCommand, Guid>, CreateEmployeeUseCase>();
            services.AddScoped<IRequestHandler<UpdateEmployeeCommand, bool>, UpdateEmployeeUseCase>();
            services.AddScoped<IRequestHandler<DeactivateEmployeeCommand, bool>, DeactivateEmployeeUseCase>();
            services.AddScoped<IRequestHandler<AssignCredentialCommand, Guid>, AssignCredentialUseCase>();
            services.AddScoped<IRequestHandler<BlockCredentialCommand, bool>, BlockCredentialUseCase>();
            services.AddScoped<IRequestHandler<RevokeCredentialCommand, bool>, RevokeCredentialUseCase>();
            services.AddScoped<IRequestHandler<RegisterAccessCommand, RegisterAccessResultDTO>, RegisterAccessUseCase>();
            services.AddScoped<IRequestHandler<CreateAccessPointCommand, Guid>, CreateAccessPointUseCase>();
            services.AddScoped<IRequestHandler<UpdateAccessPointStatusCommand, bool>, UpdateAccessPointStatusUseCase>();

            return services;
        }
    }
}
