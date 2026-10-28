using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Queries.GetEmployeesList
{
    public static class MapperExtensions
    {
        public static EmployeeListItemDTO ToListItemDTO(this Employee employee)
        {
            return new EmployeeListItemDTO
            {
                Id = employee.Id,
                IdentificationNumber = employee.IdentificationNumber,
                Names = employee.Names,
                LastNames = employee.LastNames,
                Email = employee.Email.Value,
                Position = employee.Position,
                Area = employee.Area,
                Status = employee.Status,
                CreatedAt = employee.CreatedAt
            };
        }
    }
}
