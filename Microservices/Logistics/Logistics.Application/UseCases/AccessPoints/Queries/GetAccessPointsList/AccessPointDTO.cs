using Logistics.Domain.Entities.AccessPoints;

namespace Logistics.Application.UseCases.AccessPoints.Queries.GetAccessPointsList
{
    public class AccessPointDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public AccessPointType Type { get; init; }
        public bool IsEnabled { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
