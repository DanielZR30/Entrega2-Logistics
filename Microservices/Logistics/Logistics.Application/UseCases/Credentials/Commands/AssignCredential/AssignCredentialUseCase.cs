using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.Utilities.Mediator;
using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Entities.Employees;
using Logistics.Domain.Exceptions;

namespace Logistics.Application.UseCases.Credentials.Commands.AssignCredential
{
    public class AssignCredentialUseCase : IRequestHandler<AssignCredentialCommand, Guid>
    {
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignCredentialUseCase(ICredentialsRepository credentialsRepository,
                                       IEmployeesRepository employeesRepository,
                                       IUnitOfWork unitOfWork)
        {
            _credentialsRepository = credentialsRepository;
            _employeesRepository = employeesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AssignCredentialCommand command)
        {
            Employee? employee = await _employeesRepository.GetByIdAsync(command.EmployeeId);

            if (employee is null)
            {
                throw new BussinesRuleException("El empleado al que se asignará la credencial no existe.");
            }

            string code = command.Code.Trim().ToUpperInvariant();

            Credential? existingCredential = await _credentialsRepository.GetByCodeAsync(code);

            if (existingCredential is not null)
            {
                throw new BussinesRuleException($"Ya existe una credencial con el código '{code}'.");
            }

            Credential credential = new(code, command.EmployeeId);

            await _credentialsRepository.CreateAsync(credential);
            await _unitOfWork.CommitAsync();

            return credential.Id;
        }
    }
}
