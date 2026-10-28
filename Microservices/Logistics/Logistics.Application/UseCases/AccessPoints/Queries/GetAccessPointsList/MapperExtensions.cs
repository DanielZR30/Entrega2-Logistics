using Logistics.Domain.Entities.AccessPoints;

namespace Logistics.Application.UseCases.AccessPoints.Queries.GetAccessPointsList
{
    public static class MapperExtensions
    {
        public static AccessPointDTO ToAccessPointDTO(this AccessPoint accessPoint)
        {
            return new AccessPointDTO
            {
                Id = accessPoint.Id,
                Name = accessPoint.Name,
                Description = accessPoint.Description,
                Type = accessPoint.Type,
                IsEnabled = accessPoint.IsEnabled,
                CreatedAt = accessPoint.CreatedAt
            };
        }
    }
}
