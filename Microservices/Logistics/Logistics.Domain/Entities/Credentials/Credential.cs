using Logistics.Domain.Entities.Employees;
using Logistics.Domain.Exceptions;

namespace Logistics.Domain.Entities.Credentials
{
    public sealed class Credential
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public CredentialStatus Status { get; private set; }
        public Guid EmployeeId { get; private set; }
        public Employee Employee { get; private set; } = null!;
        public DateTime AssignedAt { get; private set; }
        public DateTime? StatusChangedAt { get; private set; }

        private Credential() { }

        public Credential(string code, Guid employeeId)
        {
            ApplyCodeRules(code);
            ApplyEmployeeIdRules(employeeId);

            Id = Guid.CreateVersion7();
            Code = code.Trim().ToUpperInvariant();
            Status = CredentialStatus.Active;
            EmployeeId = employeeId;
            AssignedAt = DateTime.UtcNow;
        }

        public void Block()
        {
            if (Status != CredentialStatus.Active)
            {
                throw new BussinesRuleException("Solo las credenciales vigentes pueden bloquearse.");
            }

            Status = CredentialStatus.Blocked;
            StatusChangedAt = DateTime.UtcNow;
        }

        public void Revoke()
        {
            if (Status == CredentialStatus.Revoked)
            {
                throw new BussinesRuleException("La credencial ya se encuentra revocada.");
            }

            Status = CredentialStatus.Revoked;
            StatusChangedAt = DateTime.UtcNow;
        }

        private static void ApplyCodeRules(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new BussinesRuleException("El código de la credencial es requerido.");
            }

            string cleanCode = code.Trim();

            if (cleanCode.Length < 4 || cleanCode.Length > 32)
            {
                throw new BussinesRuleException("El código de la credencial debe tener entre 4 y 32 caracteres.");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanCode, @"^[A-Za-z0-9\-]+$"))
            {
                throw new BussinesRuleException("El código de la credencial debe contener únicamente letras, números y guiones.");
            }
        }

        private static void ApplyEmployeeIdRules(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                throw new BussinesRuleException("El empleado de la credencial es requerido.");
            }
        }
    }
}
