using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.AccessPoints;

namespace Logistics.Application.UseCases.AccessPoints.Commands.UpdateAccessPointStatus
{
    public class UpdateAccessPointStatusUseCase : IRequestHandler<UpdateAccessPointStatusCommand, bool>
    {
        private readonly IAccessPointsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAccessPointStatusUseCase(IAccessPointsRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateAccessPointStatusCommand command)
        {
            AccessPoint? accessPoint = await _repository.GetByIdAsync(command.AccessPointId);

            if (accessPoint is null)
            {
                return false;
            }

            if (command.Enable)
            {
                accessPoint.Enable();
            }
            else
            {
                accessPoint.Disable();
            }

            await _repository.UpdateAsync(accessPoint);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
