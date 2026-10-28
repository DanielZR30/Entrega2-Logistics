using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessRecords;

namespace Logistics.Application.UseCases.AccessRecords.Queries.GetAccessHistory
{
    public class GetAccessHistoryQuery : IRequest<PaginationResponse<AccessRecordDTO>>
    {
        public PaginationRequest Pagination { get; set; } = PaginationRequest.Standart();
        public Guid? EmployeeId { get; set; }
        public Guid? CredentialId { get; set; }
        public Guid? AccessPointId { get; set; }
        public AccessResult? Result { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
