using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.Contracts.Repositories
{
    public interface IEmployeesRepository : IRepository<Employee>
    {
        Task<PaginationResponse<Employee>> GetPagedListAsync(PaginationRequest request,
                                                             EmploymentStatus? status = null,
                                                             OrganizationalArea? area = null,
                                                             string? searchTerm = null,
                                                             CancellationToken cancellationToken = default);

        Task<Employee?> GetByIdWithCredentialsAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default);
    }
}
