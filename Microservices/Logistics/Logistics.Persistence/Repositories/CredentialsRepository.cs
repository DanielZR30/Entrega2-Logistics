using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.Credentials;
using Logistics.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Repositories
{
    public class CredentialsRepository : Repository<Credential>, ICredentialsRepository
    {
        private readonly DataContext _context;

        public CredentialsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Credential?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            string cleanCode = code.Trim().ToUpperInvariant();
            return await _context.Credentials
                                 .FirstOrDefaultAsync(c => c.Code == cleanCode, cancellationToken);
        }

        public async Task<Credential?> GetByCodeWithEmployeeAsync(string code, CancellationToken cancellationToken = default)
        {
            string cleanCode = code.Trim().ToUpperInvariant();
            return await _context.Credentials
                                 .Include(c => c.Employee)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Code == cleanCode, cancellationToken);
        }

        public async Task<PaginationResponse<Credential>> GetPagedListByEmployeeAsync(Guid employeeId,
                                                                                      PaginationRequest request,
                                                                                      CancellationToken cancellationToken = default)
        {
            IQueryable<Credential> query = _context.Credentials
                                                   .Include(c => c.Employee)
                                                   .AsNoTracking()
                                                   .Where(c => c.EmployeeId == employeeId)
                                                   .OrderByDescending(c => c.AssignedAt);

            return await query.ToPagedListAsync(request, cancellationToken);
        }
    }
}
