using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessRecords;

namespace Logistics.Application.Contracts.Repositories
{
    public interface IAccessRecordsRepository : IRepository<AccessRecord>
    {
        Task<PaginationResponse<AccessRecord>> GetPagedListAsync(PaginationRequest request,
                                                                 Guid? employeeId = null,
                                                                 Guid? credentialId = null,
                                                                 Guid? accessPointId = null,
                                                                 AccessResult? result = null,
                                                                 DateTime? dateFrom = null,
                                                                 DateTime? dateTo = null,
                                                                 CancellationToken cancellationToken = default);
    }
}
