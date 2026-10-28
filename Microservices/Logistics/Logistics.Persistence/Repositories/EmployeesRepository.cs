using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.Employees;
using Logistics.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Repositories
{
    public class EmployeesRepository : Repository<Employee>, IEmployeesRepository
    {
        private readonly DataContext _context;

        public EmployeesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<Employee>> GetPagedListAsync(PaginationRequest request,
                                                                          EmploymentStatus? status = null,
                                                                          OrganizationalArea? area = null,
                                                                          string? searchTerm = null,
                                                                          CancellationToken cancellationToken = default)
        {
            IQueryable<Employee> query = _context.Employees
                                                 .AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            if (area.HasValue)
            {
                query = query.Where(e => e.Area == area.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(e => e.Names.ToLower().Contains(term) ||
                                         e.LastNames.ToLower().Contains(term) ||
                                         e.IdentificationNumber.ToLower().Contains(term));
            }

            query = query.OrderBy(e => e.Names)
                         .ThenBy(e => e.LastNames);

            return await query.ToPagedListAsync(request, cancellationToken);
        }

        public async Task<Employee?> GetByIdWithCredentialsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                                 .Include(e => e.Credentials)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default)
        {
            string cleanIdentification = identificationNumber.Trim();
            return await _context.Employees.AnyAsync(e => e.IdentificationNumber == cleanIdentification, cancellationToken);
        }
    }
}
