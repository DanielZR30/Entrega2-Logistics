using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Common.ValueObjects;
using Logistics.Domain.Entities.Employees;

namespace Logistics.Application.UseCases.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeUseCase : IRequestHandler<UpdateEmployeeCommand, bool>
    {
        private readonly IEmployeesRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmployeeUseCase(IEmployeesRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateEmployeeCommand command)
        {
            Employee? employee = await _repository.GetByIdAsync(command.Id);

            if (employee is null)
            {
                return false;
            }

            Email email = Domain.Common.ValueObjects.Email.Create(command.Email);

            employee.Update(command.Names,
                            command.LastNames,
                            email,
                            command.Position,
                            command.Area);

            await _repository.UpdateAsync(employee);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
