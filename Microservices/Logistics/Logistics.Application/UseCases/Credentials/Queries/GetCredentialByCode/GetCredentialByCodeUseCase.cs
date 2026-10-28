using Logistics.Application.Contracts.Repositories;
using Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.Credentials;

namespace Logistics.Application.UseCases.Credentials.Queries.GetCredentialByCode
{
    public class GetCredentialByCodeUseCase : IRequestHandler<GetCredentialByCodeQuery, CredentialDTO?>
    {
        private readonly ICredentialsRepository _repository;

        public GetCredentialByCodeUseCase(ICredentialsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CredentialDTO?> Handle(GetCredentialByCodeQuery query)
        {
            Credential? credential = await _repository.GetByCodeWithEmployeeAsync(query.Code.Trim().ToUpperInvariant());

            if (credential is null)
            {
                return null;
            }

            return credential.ToCredentialDTO();
        }
    }
}
