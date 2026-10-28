using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessPoints;
using Logistics.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Repositories
{
    public class AccessPointsRepository : Repository<AccessPoint>, IAccessPointsRepository
    {
        private readonly DataContext _context;

        public AccessPointsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<AccessPoint>> GetPagedListAsync(PaginationRequest request,
                                                                             AccessPointType? type = null,
                                                                             bool? isEnabled = null,
                                                                             CancellationToken cancellationToken = default)
        {
            IQueryable<AccessPoint> query = _context.AccessPoints
                                                    .AsNoTracking();

            if (type.HasValue)
            {
                query = query.Where(p => p.Type == type.Value);
            }

            if (isEnabled.HasValue)
            {
                query = query.Where(p => p.IsEnabled == isEnabled.Value);
            }

            query = query.OrderBy(p => p.Name);

            return await query.ToPagedListAsync(request, cancellationToken);
        }
    }
}
