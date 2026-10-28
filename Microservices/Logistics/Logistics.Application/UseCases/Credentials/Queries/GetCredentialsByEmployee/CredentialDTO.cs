using Logistics.Domain.Entities.Credentials;

namespace Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee
{
    public class CredentialDTO
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = null!;
        public CredentialStatus Status { get; init; }
        public Guid EmployeeId { get; init; }
        public string EmployeeName { get; init; } = string.Empty;
        public DateTime AssignedAt { get; init; }
        public DateTime? StatusChangedAt { get; init; }
    }
}
