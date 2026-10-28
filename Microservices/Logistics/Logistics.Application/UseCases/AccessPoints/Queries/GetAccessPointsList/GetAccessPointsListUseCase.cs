using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessPoints;

namespace Logistics.Application.UseCases.AccessPoints.Queries.GetAccessPointsList
{
    public class GetAccessPointsListUseCase : IRequestHandler<GetAccessPointsListQuery, PaginationResponse<AccessPointDTO>>
    {
        private readonly IAccessPointsRepository _repository;

        public GetAccessPointsListUseCase(IAccessPointsRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginationResponse<AccessPointDTO>> Handle(GetAccessPointsListQuery query)
        {
            PaginationRequest pagination = query.Pagination;

            PaginationResponse<AccessPoint> response = await _repository.GetPagedListAsync(
                pagination,
                query.Type,
                query.IsEnabled);

            List<AccessPointDTO> items = response.Items.Select(p => p.ToAccessPointDTO()).ToList();

            return PaginationResponse<AccessPointDTO>.Create(items, response.TotalCount, pagination);
        }
    }
}
