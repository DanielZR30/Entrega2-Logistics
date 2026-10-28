using Logistics.Application.Utilities.Mediator;

namespace Logistics.Application.UseCases.AccessPoints.Commands.UpdateAccessPointStatus
{
    public class UpdateAccessPointStatusCommand : IRequest<bool>
    {
        public Guid AccessPointId { get; set; }
        public bool Enable { get; set; }

        public UpdateAccessPointStatusCommand(Guid accessPointId, bool enable)
        {
            AccessPointId = accessPointId;
            Enable = enable;
        }

        public UpdateAccessPointStatusCommand() { }
    }
}
