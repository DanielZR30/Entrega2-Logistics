using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Names { get; set; } = string.Empty;
        public string LastNames { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public OrganizationalArea Area { get; set; }
    }
}
