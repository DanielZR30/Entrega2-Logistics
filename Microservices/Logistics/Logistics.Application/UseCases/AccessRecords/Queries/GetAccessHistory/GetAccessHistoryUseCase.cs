using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessRecords;

namespace Logistics.Application.UseCases.AccessRecords.Queries.GetAccessHistory
{
    public class GetAccessHistoryUseCase : IRequestHandler<GetAccessHistoryQuery, PaginationResponse<AccessRecordDTO>>
    {
        private readonly IAccessRecordsRepository _repository;

        public GetAccessHistoryUseCase(IAccessRecordsRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginationResponse<AccessRecordDTO>> Handle(GetAccessHistoryQuery query)
        {
            PaginationRequest pagination = query.Pagination;

            PaginationResponse<AccessRecord> response = await _repository.GetPagedListAsync(
                pagination,
                query.EmployeeId,
                query.CredentialId,
                query.AccessPointId,
                query.Result,
                query.DateFrom,
                query.DateTo);

            List<AccessRecordDTO> items = response.Items.Select(r => r.ToAccessRecordDTO()).ToList();

            return PaginationResponse<AccessRecordDTO>.Create(items, response.TotalCount, pagination);
        }
    }
}
