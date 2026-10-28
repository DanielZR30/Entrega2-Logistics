using Logistics.Application.Utilities.Mediator;

namespace Logistics.Application.UseCases.Employees.Commands.DeactivateEmployee
{
    public class DeactivateEmployeeCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeactivateEmployeeCommand(Guid id)
        {
            Id = id;
        }

        public DeactivateEmployeeCommand() { }
    }
}
