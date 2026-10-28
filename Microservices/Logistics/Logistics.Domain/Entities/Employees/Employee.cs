using Logistics.Domain.Common.ValueObjects;
using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Exceptions;

namespace Logistics.Domain.Entities.Employees
{
    public sealed class Employee
    {
        public Guid Id { get; private set; }
        public string IdentificationNumber { get; private set; } = null!;
        public string Names { get; private set; } = null!;
        public string LastNames { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public string Position { get; private set; } = null!;
        public OrganizationalArea Area { get; private set; }
        public EmploymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public ICollection<Credential> Credentials { get; private set; } = new List<Credential>();

        private Employee() { }

        public Employee(string identificationNumber,
                        string names,
                        string lastNames,
                        Email email,
                        string position,
                        OrganizationalArea area)
        {
            ApplyIdentificationNumberRules(identificationNumber);
            ApplyNamesRules(names);
            ApplyLastNamesRules(lastNames);
            ApplyEmailRules(email);
            ApplyPositionRules(position);
            ApplyAreaRules(area);

            Id = Guid.CreateVersion7();
            IdentificationNumber = identificationNumber.Trim();
            Names = names.Trim();
            LastNames = lastNames.Trim();
            Email = email;
            Position = position.Trim();
            Area = area;
            Status = EmploymentStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string names,
                           string lastNames,
                           Email email,
                           string position,
                           OrganizationalArea area)
        {
            ApplyNamesRules(names);
            ApplyLastNamesRules(lastNames);
            ApplyEmailRules(email);
            ApplyPositionRules(position);
            ApplyAreaRules(area);

            Names = names.Trim();
            LastNames = lastNames.Trim();
            Email = email;
            Position = position.Trim();
            Area = area;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (Status == EmploymentStatus.Inactive)
            {
                throw new BussinesRuleException("El empleado ya se encuentra inactivo.");
            }

            Status = EmploymentStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ApplyIdentificationNumberRules(string identificationNumber)
        {
            if (string.IsNullOrWhiteSpace(identificationNumber))
            {
                throw new BussinesRuleException("La identificación del empleado es requerida.");
            }

            string cleanIdentification = identificationNumber.Trim();

            if (cleanIdentification.Length < 4 || cleanIdentification.Length > 20)
            {
                throw new BussinesRuleException("La identificación del empleado debe tener entre 4 y 20 caracteres.");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanIdentification, @"^[A-Za-z0-9\-]+$"))
            {
                throw new BussinesRuleException("La identificación del empleado debe contener únicamente letras, números y guiones.");
            }
        }

        private static void ApplyNamesRules(string names)
        {
            if (string.IsNullOrWhiteSpace(names))
            {
                throw new BussinesRuleException("Los nombres del empleado son requeridos.");
            }

            if (names.Trim().Length < 2)
            {
                throw new BussinesRuleException("Los nombres del empleado deben tener al menos 2 caracteres.");
            }

            if (names.Trim().Length > 100)
            {
                throw new BussinesRuleException("Los nombres del empleado deben tener máximo 100 caracteres.");
            }
        }

        private static void ApplyLastNamesRules(string lastNames)
        {
            if (string.IsNullOrWhiteSpace(lastNames))
            {
                throw new BussinesRuleException("Los apellidos del empleado son requeridos.");
            }

            if (lastNames.Trim().Length < 2)
            {
                throw new BussinesRuleException("Los apellidos del empleado deben tener al menos 2 caracteres.");
            }

            if (lastNames.Trim().Length > 100)
            {
                throw new BussinesRuleException("Los apellidos del empleado deben tener máximo 100 caracteres.");
            }
        }

        private static void ApplyEmailRules(Email email)
        {
            if (email is null)
            {
                throw new BussinesRuleException("El correo electrónico del empleado es requerido.");
            }
        }

        private static void ApplyPositionRules(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
            {
                throw new BussinesRuleException("El cargo del empleado es requerido.");
            }

            if (position.Trim().Length < 2)
            {
                throw new BussinesRuleException("El cargo del empleado debe tener al menos 2 caracteres.");
            }

            if (position.Trim().Length > 100)
            {
                throw new BussinesRuleException("El cargo del empleado debe tener máximo 100 caracteres.");
            }
        }

        private static void ApplyAreaRules(OrganizationalArea area)
        {
            if (!Enum.IsDefined(area))
            {
                throw new BussinesRuleException("El área organizacional del empleado no es válida.");
            }
        }
    }
}
