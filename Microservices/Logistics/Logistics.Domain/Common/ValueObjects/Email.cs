using Logistics.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Logistics.Domain.Common.ValueObjects
{
    public sealed class Email : IEquatable<Email>
    {
        public string Value { get; private set; } = null!;

        private Email() { }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BussinesRuleException("El correo electrónico es requerido.");
            }

            string cleanEmail = value.Trim();

            if (cleanEmail.Length > 256)
            {
                throw new BussinesRuleException("El correo electrónico debe tener máximo 256 caracteres.");
            }

            if (!Regex.IsMatch(cleanEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new BussinesRuleException("El formato del correo electrónico no es válido.");
            }

            return new Email(cleanEmail);
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Email other && Equals(other);
        }

        public bool Equals(Email? other)
        {
            if (other is null) return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value.ToLowerInvariant().GetHashCode();
        }

        public static bool operator ==(Email? left, Email? right) => Equals(left, right);
        public static bool operator !=(Email? left, Email? right) => !Equals(left, right);
    }
}
