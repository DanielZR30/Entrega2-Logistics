using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommand : IRequest<Guid>
    {
        public string IdentificationNumber { get; set; } = string.Empty;
        public string Names { get; set; } = string.Empty;
        public string LastNames { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public OrganizationalArea Area { get; set; }
    }
}
