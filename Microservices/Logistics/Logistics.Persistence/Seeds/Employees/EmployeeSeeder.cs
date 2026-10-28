using Logistics.Domain.Common.ValueObjects;
using Logistics.Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Seeds.Employees
{
    public class EmployeeSeeder : IDataSeeder
    {
        private readonly DataContext _context;

        public EmployeeSeeder(DataContext context)
        {
            _context = context;
        }

        public int Order => 2;

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.Employees.AnyAsync(cancellationToken))
            {
                return;
            }

            Employee pedro = new("1122334455", "Pedro", "Sánchez", Email.Create("pedro.sanchez@logistics.com"), "Operario de Bodega", OrganizationalArea.Warehouse);
            pedro.Deactivate();

            List<Employee> employees =
            [
                new Employee("1020304050", "Daniel", "Zapata", Email.Create("daniel.zapata@logistics.com"), "Operario de Bodega", OrganizationalArea.Warehouse),
                new Employee("5566778899", "María", "Gómez", Email.Create("maria.gomez@logistics.com"), "Auxiliar Administrativa", OrganizationalArea.Administration),
                new Employee("7788990011", "Carlos", "Ruiz", Email.Create("carlos.ruiz@logistics.com"), "Guardia de Seguridad", OrganizationalArea.Security),
                new Employee("8899001122", "Laura", "Martínez", Email.Create("laura.martinez@logistics.com"), "Coordinadora de Operaciones", OrganizationalArea.Operations),
                new Employee("9900112233", "Andrés", "Molina", Email.Create("andres.molina@logistics.com"), "Técnico de Mantenimiento", OrganizationalArea.Maintenance),
                pedro
            ];

            await _context.Employees.AddRangeAsync(employees, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
