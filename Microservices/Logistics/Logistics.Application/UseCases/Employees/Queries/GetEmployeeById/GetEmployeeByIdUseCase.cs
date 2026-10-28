using Logistics.Application.Contracts.Repositories;
using Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Queries.GetEmployeeById
{
    public class GetEmployeeByIdUseCase : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailsDTO?>
    {
        private readonly IEmployeesRepository _repository;

        public GetEmployeeByIdUseCase(IEmployeesRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeDetailsDTO?> Handle(GetEmployeeByIdQuery query)
        {
            Employee? employee = await _repository.GetByIdWithCredentialsAsync(query.Id);

            if (employee is null)
            {
                return null;
            }

            return new EmployeeDetailsDTO
            {
                Id = employee.Id,
                IdentificationNumber = employee.IdentificationNumber,
                Names = employee.Names,
                LastNames = employee.LastNames,
                Email = employee.Email.Value,
                Position = employee.Position,
                Area = employee.Area,
                Status = employee.Status,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt,
                Credentials = employee.Credentials.Select(c => c.ToCredentialDTO()).ToList()
            };
        }
    }
}
