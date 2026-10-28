using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessRecords;
using Logistics.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Repositories
{
    public class AccessRecordsRepository : Repository<AccessRecord>, IAccessRecordsRepository
    {
        private readonly DataContext _context;

        public AccessRecordsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<AccessRecord>> GetPagedListAsync(PaginationRequest request,
                                                                              Guid? employeeId = null,
                                                                              Guid? credentialId = null,
                                                                              Guid? accessPointId = null,
                                                                              AccessResult? result = null,
                                                                              DateTime? dateFrom = null,
                                                                              DateTime? dateTo = null,
                                                                              CancellationToken cancellationToken = default)
        {
            IQueryable<AccessRecord> query = _context.AccessRecords
                                                     .Include(r => r.Credential)
                                                     .ThenInclude(c => c!.Employee)
                                                     .Include(r => r.AccessPoint)
                                                     .AsNoTracking();

            if (employeeId.HasValue)
            {
                query = query.Where(r => r.Credential!.EmployeeId == employeeId.Value);
            }

            if (credentialId.HasValue)
            {
                query = query.Where(r => r.CredentialId == credentialId.Value);
            }

            if (accessPointId.HasValue)
            {
                query = query.Where(r => r.AccessPointId == accessPointId.Value);
            }

            if (result.HasValue)
            {
                query = query.Where(r => r.Result == result.Value);
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(r => r.OccurredAt >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(r => r.OccurredAt <= dateTo.Value);
            }

            query = query.OrderByDescending(r => r.OccurredAt);

            return await query.ToPagedListAsync(request, cancellationToken);
        }
    }
}
