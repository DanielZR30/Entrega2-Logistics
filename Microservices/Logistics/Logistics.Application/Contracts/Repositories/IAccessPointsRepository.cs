using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessPoints;

namespace Logistics.Application.Contracts.Repositories
{
    public interface IAccessPointsRepository : IRepository<AccessPoint>
    {
        Task<PaginationResponse<AccessPoint>> GetPagedListAsync(PaginationRequest request,
                                                                AccessPointType? type = null,
                                                                bool? isEnabled = null,
                                                                CancellationToken cancellationToken = default);
    }
}
