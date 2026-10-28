using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.Credentials;

namespace Logistics.Application.UseCases.Credentials.Commands.BlockCredential
{
    public class BlockCredentialUseCase : IRequestHandler<BlockCredentialCommand, bool>
    {
        private readonly ICredentialsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public BlockCredentialUseCase(ICredentialsRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(BlockCredentialCommand command)
        {
            Credential? credential = await _repository.GetByIdAsync(command.CredentialId);

            if (credential is null)
            {
                return false;
            }

            credential.Block();

            await _repository.UpdateAsync(credential);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
