using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.Credentials;

namespace Logistics.Application.Contracts.Repositories
{
    public interface ICredentialsRepository : IRepository<Credential>
    {
        Task<Credential?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<Credential?> GetByCodeWithEmployeeAsync(string code, CancellationToken cancellationToken = default);

        Task<PaginationResponse<Credential>> GetPagedListByEmployeeAsync(Guid employeeId,
                                                                         PaginationRequest request,
                                                                         CancellationToken cancellationToken = default);
    }
}
