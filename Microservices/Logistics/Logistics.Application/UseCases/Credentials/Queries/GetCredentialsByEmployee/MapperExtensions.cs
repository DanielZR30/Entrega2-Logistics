using Logistics.Domain.Entities.Credentials;

namespace Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee
{
    public static class MapperExtensions
    {
        public static CredentialDTO ToCredentialDTO(this Credential credential)
        {
            return new CredentialDTO
            {
                Id = credential.Id,
                Code = credential.Code,
                Status = credential.Status,
                EmployeeId = credential.EmployeeId,
                EmployeeName = credential.Employee is null
                    ? string.Empty
                    : $"{credential.Employee.Names} {credential.Employee.LastNames}",
                AssignedAt = credential.AssignedAt,
                StatusChangedAt = credential.StatusChangedAt
            };
        }
    }
}
