using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.AccessRecords;

namespace Logistics.Application.UseCases.AccessRecords.Commands.RegisterAccess
{
    public class RegisterAccessCommand : IRequest<RegisterAccessResultDTO>
    {
        public string CredentialCode { get; set; } = string.Empty;
        public Guid AccessPointId { get; set; }
        public MovementType MovementType { get; set; }
    }
}
