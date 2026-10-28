using Logistics.Application.Utilities.Mediator;

namespace Logistics.Application.UseCases.Credentials.Commands.AssignCredential
{
    public class AssignCredentialCommand : IRequest<Guid>
    {
        public string Code { get; set; } = string.Empty;
        public Guid EmployeeId { get; set; }
    }
}
