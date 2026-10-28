using Logistics.Domain.Entities.AccessRecords;

namespace Logistics.Application.UseCases.AccessRecords.Queries.GetAccessHistory
{
    public static class MapperExtensions
    {
        public static AccessRecordDTO ToAccessRecordDTO(this AccessRecord record)
        {
            return new AccessRecordDTO
            {
                Id = record.Id,
                CredentialId = record.CredentialId,
                CredentialCode = record.Credential?.Code ?? string.Empty,
                EmployeeId = record.Credential?.EmployeeId ?? Guid.Empty,
                EmployeeName = record.Credential?.Employee is null
                    ? string.Empty
                    : $"{record.Credential.Employee.Names} {record.Credential.Employee.LastNames}",
                AccessPointId = record.AccessPointId,
                AccessPointName = record.AccessPoint?.Name ?? string.Empty,
                AccessPointType = record.AccessPoint?.Type ?? default,
                MovementType = record.MovementType,
                Result = record.Result,
                DeniedReason = record.DeniedReason,
                OccurredAt = record.OccurredAt
            };
        }
    }
}
