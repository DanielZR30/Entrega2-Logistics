using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Common.ValueObjects;
using Logistics.Domain.Entities.Employees;
using Logistics.Domain.Exceptions;

namespace Logistics.Application.UseCases.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeUseCase : IRequestHandler<CreateEmployeeCommand, Guid>
    {
        private readonly IEmployeesRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEmployeeUseCase(IEmployeesRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateEmployeeCommand command)
        {
            bool existsIdentification = await _repository.ExistsByIdentificationAsync(command.IdentificationNumber.Trim());

            if (existsIdentification)
            {
                throw new BussinesRuleException($"Ya existe un empleado con la identificación '{command.IdentificationNumber.Trim()}'.");
            }

            Email email = Domain.Common.ValueObjects.Email.Create(command.Email);

            Employee employee = new(command.IdentificationNumber,
                                    command.Names,
                                    command.LastNames,
                                    email,
                                    command.Position,
                                    command.Area);

            await _repository.CreateAsync(employee);
            await _unitOfWork.CommitAsync();

            return employee.Id;
        }
    }
}
