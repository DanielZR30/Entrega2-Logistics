using Logistics.Application.Utilities.Mediator;

namespace Logistics.Application.UseCases.Employees.Queries.GetEmployeeById
{
    public class GetEmployeeByIdQuery : IRequest<EmployeeDetailsDTO?>
    {
        public Guid Id { get; set; }

        public GetEmployeeByIdQuery(Guid id)
        {
            Id = id;
        }

        public GetEmployeeByIdQuery() { }
    }
}
