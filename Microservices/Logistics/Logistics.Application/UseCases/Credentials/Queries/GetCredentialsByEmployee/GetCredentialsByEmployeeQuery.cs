using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;

namespace Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee
{
    public class GetCredentialsByEmployeeQuery : IRequest<PaginationResponse<CredentialDTO>>
    {
        public Guid EmployeeId { get; set; }
        public PaginationRequest Pagination { get; set; } = PaginationRequest.Standart();

        public GetCredentialsByEmployeeQuery(Guid employeeId, PaginationRequest? pagination = null)
        {
            EmployeeId = employeeId;
            if (pagination is not null)
            {
                Pagination = pagination;
            }
        }

        public GetCredentialsByEmployeeQuery() { }
    }
}
