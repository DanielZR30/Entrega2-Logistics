using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Queries.GetEmployeesList
{
    public class GetEmployeesListUseCase : IRequestHandler<GetEmployeesListQuery, PaginationResponse<EmployeeListItemDTO>>
    {
        private readonly IEmployeesRepository _repository;

        public GetEmployeesListUseCase(IEmployeesRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginationResponse<EmployeeListItemDTO>> Handle(GetEmployeesListQuery query)
        {
            PaginationRequest pagination = query.Pagination;

            PaginationResponse<Employee> response = await _repository.GetPagedListAsync(
                pagination,
                query.Status,
                query.Area,
                query.SearchTerm);

            List<EmployeeListItemDTO> items = response.Items.Select(e => e.ToListItemDTO()).ToList();

            return PaginationResponse<EmployeeListItemDTO>.Create(items, response.TotalCount, pagination);
        }
    }
}
