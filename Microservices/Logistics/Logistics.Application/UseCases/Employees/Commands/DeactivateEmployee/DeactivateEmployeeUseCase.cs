using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Commands.DeactivateEmployee
{
    public class DeactivateEmployeeUseCase : IRequestHandler<DeactivateEmployeeCommand, bool>
    {
        private readonly IEmployeesRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateEmployeeUseCase(IEmployeesRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeactivateEmployeeCommand command)
        {
            Employee? employee = await _repository.GetByIdAsync(command.Id);

            if (employee is null)
            {
                return false;
            }

            employee.Deactivate();

            await _repository.UpdateAsync(employee);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
