using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Entities.AccessRecords;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.AccessRecords.Queries.GetAccessHistory
{
    public class AccessRecordDTO
    {
        public Guid Id { get; init; }
        public Guid CredentialId { get; init; }
        public string CredentialCode { get; init; } = null!;
        public Guid EmployeeId { get; init; }
        public string EmployeeName { get; init; } = string.Empty;
        public Guid AccessPointId { get; init; }
        public string AccessPointName { get; init; } = null!;
        public AccessPointType AccessPointType { get; init; }
        public MovementType MovementType { get; init; }
        public AccessResult Result { get; init; }
        public DenialReason? DeniedReason { get; init; }
        public DateTime OccurredAt { get; init; }
    }
}
