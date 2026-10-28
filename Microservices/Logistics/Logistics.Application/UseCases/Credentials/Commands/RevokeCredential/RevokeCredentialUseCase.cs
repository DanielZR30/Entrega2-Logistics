using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.Credentials;

namespace Logistics.Application.UseCases.Credentials.Commands.RevokeCredential
{
    public class RevokeCredentialUseCase : IRequestHandler<RevokeCredentialCommand, bool>
    {
        private readonly ICredentialsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RevokeCredentialUseCase(ICredentialsRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RevokeCredentialCommand command)
        {
            Credential? credential = await _repository.GetByIdAsync(command.CredentialId);

            if (credential is null)
            {
                return false;
            }

            credential.Revoke();

            await _repository.UpdateAsync(credential);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
