using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Entities.AccessRecords;
using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.AccessRecords.Commands.RegisterAccess
{
    public class RegisterAccessUseCase : IRequestHandler<RegisterAccessCommand, RegisterAccessResultDTO>
    {
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IAccessPointsRepository _accessPointsRepository;
        private readonly IAccessRecordsRepository _accessRecordsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterAccessUseCase(ICredentialsRepository credentialsRepository,
                                     IAccessPointsRepository accessPointsRepository,
                                     IAccessRecordsRepository accessRecordsRepository,
                                     IUnitOfWork unitOfWork)
        {
            _credentialsRepository = credentialsRepository;
            _accessPointsRepository = accessPointsRepository;
            _accessRecordsRepository = accessRecordsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterAccessResultDTO> Handle(RegisterAccessCommand command)
        {
            string code = command.CredentialCode.Trim().ToUpperInvariant();

            Credential? credential = await _credentialsRepository.GetByCodeWithEmployeeAsync(code);

            if (credential is null)
            {
                return new RegisterAccessResultDTO
                {
                    Registered = false,
                    Message = $"No existe una credencial con el código '{code}'."
                };
            }

            AccessPoint? accessPoint = await _accessPointsRepository.GetByIdAsync(command.AccessPointId);

            if (accessPoint is null)
            {
                return new RegisterAccessResultDTO
                {
                    Registered = false,
                    Message = "El punto de acceso indicado no existe."
                };
            }

            AccessRecord record;
            string message;

            if (credential.Employee is null || credential.Employee.Status == EmploymentStatus.Inactive)
            {
                record = AccessRecord.CreateDenied(credential.Id, accessPoint.Id, command.MovementType, DenialReason.EmployeeInactive);
                message = "Acceso rechazado: el empleado se encuentra inactivo.";
            }
            else if (credential.Status == CredentialStatus.Blocked)
            {
                record = AccessRecord.CreateDenied(credential.Id, accessPoint.Id, command.MovementType, DenialReason.CredentialBlocked);
                message = "Acceso rechazado: la credencial se encuentra bloqueada.";
            }
            else if (credential.Status == CredentialStatus.Revoked)
            {
                record = AccessRecord.CreateDenied(credential.Id, accessPoint.Id, command.MovementType, DenialReason.CredentialRevoked);
                message = "Acceso rechazado: la credencial se encuentra revocada.";
            }
            else if (!accessPoint.IsEnabled)
            {
                record = AccessRecord.CreateDenied(credential.Id, accessPoint.Id, command.MovementType, DenialReason.AccessPointDisabled);
                message = "Acceso rechazado: el punto de acceso se encuentra deshabilitado.";
            }
            else
            {
                record = AccessRecord.CreateAuthorized(credential.Id, accessPoint.Id, command.MovementType);
                message = "Acceso autorizado.";
            }

            await _accessRecordsRepository.CreateAsync(record);
            await _unitOfWork.CommitAsync();

            return new RegisterAccessResultDTO
            {
                Registered = true,
                RecordId = record.Id,
                Result = record.Result,
                DeniedReason = record.DeniedReason,
                Message = message,
                OccurredAt = record.OccurredAt
            };
        }
    }
}
