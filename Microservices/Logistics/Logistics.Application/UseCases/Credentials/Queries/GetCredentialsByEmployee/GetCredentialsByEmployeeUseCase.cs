using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.Credentials;

namespace Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee
{
    public class GetCredentialsByEmployeeUseCase : IRequestHandler<GetCredentialsByEmployeeQuery, PaginationResponse<CredentialDTO>>
    {
        private readonly ICredentialsRepository _repository;

        public GetCredentialsByEmployeeUseCase(ICredentialsRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginationResponse<CredentialDTO>> Handle(GetCredentialsByEmployeeQuery query)
        {
            PaginationRequest pagination = query.Pagination;

            PaginationResponse<Credential> response = await _repository.GetPagedListByEmployeeAsync(
                query.EmployeeId,
                pagination);

            List<CredentialDTO> items = response.Items.Select(c => c.ToCredentialDTO()).ToList();

            return PaginationResponse<CredentialDTO>.Create(items, response.TotalCount, pagination);
        }
    }
}
