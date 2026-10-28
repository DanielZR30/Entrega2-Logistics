using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Queries.GetEmployeesList
{
    public class EmployeeListItemDTO
    {
        public Guid Id { get; init; }
        public string IdentificationNumber { get; init; } = null!;
        public string Names { get; init; } = null!;
        public string LastNames { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Position { get; init; } = null!;
        public OrganizationalArea Area { get; init; }
        public EmploymentStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
