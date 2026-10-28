using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessPoints;

namespace Logistics.Application.UseCases.AccessPoints.Queries.GetAccessPointsList
{
    public class GetAccessPointsListQuery : IRequest<PaginationResponse<AccessPointDTO>>
    {
        public PaginationRequest Pagination { get; set; } = PaginationRequest.Standart();
        public AccessPointType? Type { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
