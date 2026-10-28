using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Seeds.Credentials
{
    public class CredentialSeeder : IDataSeeder
    {
        private readonly DataContext _context;

        public CredentialSeeder(DataContext context)
        {
            _context = context;
        }

        public int Order => 3;

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.Credentials.AnyAsync(cancellationToken))
            {
                return;
            }

            List<Employee> employees = await _context.Employees.ToListAsync(cancellationToken);

            Employee daniel = employees.First(e => e.IdentificationNumber == "1020304050");
            Employee maria = employees.First(e => e.IdentificationNumber == "5566778899");
            Employee carlos = employees.First(e => e.IdentificationNumber == "7788990011");
            Employee laura = employees.First(e => e.IdentificationNumber == "8899001122");
            Employee andres = employees.First(e => e.IdentificationNumber == "9900112233");
            Employee pedro = employees.First(e => e.IdentificationNumber == "1122334455");

            Credential credencialAndres = new("CRD-0005", andres.Id);
            credencialAndres.Block();

            Credential credencialPedro = new("CRD-0006", pedro.Id);
            credencialPedro.Revoke();

            List<Credential> credentials =
            [
                new Credential("CRD-0001", daniel.Id),
                new Credential("CRD-0002", maria.Id),
                new Credential("CRD-0003", carlos.Id),
                new Credential("CRD-0004", laura.Id),
                credencialAndres,
                credencialPedro
            ];

            await _context.Credentials.AddRangeAsync(credentials, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
