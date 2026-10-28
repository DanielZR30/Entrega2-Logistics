using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.AccessPoints;

namespace Logistics.Application.UseCases.AccessPoints.Commands.CreateAccessPoint
{
    public class CreateAccessPointUseCase : IRequestHandler<CreateAccessPointCommand, Guid>
    {
        private readonly IAccessPointsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAccessPointUseCase(IAccessPointsRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateAccessPointCommand command)
        {
            AccessPoint accessPoint = new(command.Name,
                                          command.Type,
                                          command.Description,
                                          command.IsEnabled);

            await _repository.CreateAsync(accessPoint);
            await _unitOfWork.CommitAsync();

            return accessPoint.Id;
        }
    }
}
