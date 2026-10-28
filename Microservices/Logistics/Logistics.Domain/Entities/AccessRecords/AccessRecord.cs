using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Exceptions;

namespace Logistics.Domain.Entities.AccessRecords
{
    public sealed class AccessRecord
    {
        public Guid Id { get; private set; }
        public Guid CredentialId { get; private set; }
        public Credential Credential { get; private set; } = null!;
        public Guid AccessPointId { get; private set; }
        public AccessPoint AccessPoint { get; private set; } = null!;
        public MovementType MovementType { get; private set; }
        public AccessResult Result { get; private set; }
        public DenialReason? DeniedReason { get; private set; }
        public DateTime OccurredAt { get; private set; }

        private AccessRecord() { }

        public AccessRecord(Guid credentialId,
                            Guid accessPointId,
                            MovementType movementType,
                            AccessResult result,
                            DenialReason? deniedReason = null)
        {
            ApplyCredentialIdRules(credentialId);
            ApplyAccessPointIdRules(accessPointId);
            ApplyMovementTypeRules(movementType);
            ApplyResultRules(result, deniedReason);

            Id = Guid.CreateVersion7();
            CredentialId = credentialId;
            AccessPointId = accessPointId;
            MovementType = movementType;
            Result = result;
            DeniedReason = result == AccessResult.Denied ? deniedReason : null;
            OccurredAt = DateTime.UtcNow;
        }

        public static AccessRecord CreateAuthorized(Guid credentialId,
                                                    Guid accessPointId,
                                                    MovementType movementType)
        {
            return new AccessRecord(credentialId, accessPointId, movementType, AccessResult.Authorized);
        }

        public static AccessRecord CreateDenied(Guid credentialId,
                                                Guid accessPointId,
                                                MovementType movementType,
                                                DenialReason deniedReason)
        {
            return new AccessRecord(credentialId, accessPointId, movementType, AccessResult.Denied, deniedReason);
        }

        private static void ApplyCredentialIdRules(Guid credentialId)
        {
            if (credentialId == Guid.Empty)
            {
                throw new BussinesRuleException("La credencial del movimiento es requerida.");
            }
        }

        private static void ApplyAccessPointIdRules(Guid accessPointId)
        {
            if (accessPointId == Guid.Empty)
            {
                throw new BussinesRuleException("El punto de acceso del movimiento es requerido.");
            }
        }

        private static void ApplyMovementTypeRules(MovementType movementType)
        {
            if (!Enum.IsDefined(movementType))
            {
                throw new BussinesRuleException("El tipo de movimiento no es válido.");
            }
        }

        private static void ApplyResultRules(AccessResult result, DenialReason? deniedReason)
        {
            if (!Enum.IsDefined(result))
            {
                throw new BussinesRuleException("El resultado del acceso no es válido.");
            }

            if (result == AccessResult.Denied && deniedReason is null)
            {
                throw new BussinesRuleException("El motivo de rechazo es requerido cuando el acceso es denegado.");
            }

            if (result == AccessResult.Authorized && deniedReason is not null)
            {
                throw new BussinesRuleException("Un acceso autorizado no puede tener motivo de rechazo.");
            }
        }
    }
}
