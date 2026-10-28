using Logistics.Domain.Entities.AccessRecords;
using Logistics.Domain.Exceptions;

namespace Logistics.Domain.Entities.AccessPoints
{
    public sealed class AccessPoint
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public AccessPointType Type { get; private set; }
        public bool IsEnabled { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public ICollection<AccessRecord> AccessRecords { get; private set; } = new List<AccessRecord>();

        private AccessPoint() { }

        public AccessPoint(string name,
                           AccessPointType type,
                           string? description = null,
                           bool isEnabled = true)
        {
            ApplyNameRules(name);
            ApplyTypeRules(type);
            ApplyDescriptionRules(description);

            Id = Guid.CreateVersion7();
            Name = name.Trim();
            Type = type;
            Description = description?.Trim();
            IsEnabled = isEnabled;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name,
                           AccessPointType type,
                           string? description = null)
        {
            ApplyNameRules(name);
            ApplyTypeRules(type);
            ApplyDescriptionRules(description);

            Name = name.Trim();
            Type = type;
            Description = description?.Trim();
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        private static void ApplyNameRules(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BussinesRuleException("El nombre del punto de acceso es requerido.");
            }

            if (name.Trim().Length < 2)
            {
                throw new BussinesRuleException("El nombre del punto de acceso debe tener al menos 2 caracteres.");
            }

            if (name.Trim().Length > 64)
            {
                throw new BussinesRuleException("El nombre del punto de acceso debe tener máximo 64 caracteres.");
            }
        }

        private static void ApplyTypeRules(AccessPointType type)
        {
            if (!Enum.IsDefined(type))
            {
                throw new BussinesRuleException("El tipo del punto de acceso no es válido.");
            }
        }

        private static void ApplyDescriptionRules(string? description)
        {
            if (description is not null && description.Trim().Length > 512)
            {
                throw new BussinesRuleException("La descripción del punto de acceso debe tener máximo 512 caracteres.");
            }
        }
    }
}
