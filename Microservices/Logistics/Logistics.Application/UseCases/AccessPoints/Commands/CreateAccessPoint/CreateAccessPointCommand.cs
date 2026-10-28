using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.AccessPoints;

namespace Logistics.Application.UseCases.AccessPoints.Commands.CreateAccessPoint
{
    public class CreateAccessPointCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public AccessPointType Type { get; set; }
        public string? Description { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
