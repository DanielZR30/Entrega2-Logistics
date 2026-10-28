using Logistics.Domain.Entities.AccessRecords;

namespace Logistics.Application.UseCases.AccessRecords.Commands.RegisterAccess
{
    public class RegisterAccessResultDTO
    {
        public bool Registered { get; init; }
        public Guid? RecordId { get; init; }
        public AccessResult? Result { get; init; }
        public DenialReason? DeniedReason { get; init; }
        public string Message { get; init; } = string.Empty;
        public DateTime? OccurredAt { get; init; }
    }
}
