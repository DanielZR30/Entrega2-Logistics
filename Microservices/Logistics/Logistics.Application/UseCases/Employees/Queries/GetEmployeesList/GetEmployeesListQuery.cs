using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Queries.GetEmployeesList
{
    public class GetEmployeesListQuery : IRequest<PaginationResponse<EmployeeListItemDTO>>
    {
        public PaginationRequest Pagination { get; set; } = PaginationRequest.Standart();
        public EmploymentStatus? Status { get; set; }
        public OrganizationalArea? Area { get; set; }
        public string? SearchTerm { get; set; }
    }
}
