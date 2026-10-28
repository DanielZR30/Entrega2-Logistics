using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Entities.AccessRecords;
using Logistics.Domain.Entities.Credentials;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Seeds.AccessRecords
{
    public class AccessRecordSeeder : IDataSeeder
    {
        private readonly DataContext _context;

        public AccessRecordSeeder(DataContext context)
        {
            _context = context;
        }

        public int Order => 4;

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.AccessRecords.AnyAsync(cancellationToken))
            {
                return;
            }

            List<Credential> credentials = await _context.Credentials.ToListAsync(cancellationToken);
            List<AccessPoint> accessPoints = await _context.AccessPoints.ToListAsync(cancellationToken);

            Credential credencialDaniel = credentials.First(c => c.Code == "CRD-0001");
            Credential credencialMaria = credentials.First(c => c.Code == "CRD-0002");
            Credential credencialCarlos = credentials.First(c => c.Code == "CRD-0003");
            Credential credencialAndres = credentials.First(c => c.Code == "CRD-0005");
            Credential credencialPedro = credentials.First(c => c.Code == "CRD-0006");

            AccessPoint recepcion = accessPoints.First(p => p.Name == "Recepción Principal");
            AccessPoint bodega = accessPoints.First(p => p.Name == "Bodega Central");
            AccessPoint servidores = accessPoints.First(p => p.Name == "Área Restringida - Servidores");
            AccessPoint puertaOccidental = accessPoints.First(p => p.Name == "Puerta Occidental");

            List<AccessRecord> records =
            [
                AccessRecord.CreateAuthorized(credencialDaniel.Id, bodega.Id, MovementType.Entry),
                AccessRecord.CreateAuthorized(credencialDaniel.Id, bodega.Id, MovementType.Exit),
                AccessRecord.CreateAuthorized(credencialMaria.Id, recepcion.Id, MovementType.Entry),
                AccessRecord.CreateAuthorized(credencialMaria.Id, recepcion.Id, MovementType.Exit),
                AccessRecord.CreateAuthorized(credencialCarlos.Id, recepcion.Id, MovementType.Entry),
                AccessRecord.CreateAuthorized(credencialCarlos.Id, servidores.Id, MovementType.Entry),
                AccessRecord.CreateDenied(credencialAndres.Id, bodega.Id, MovementType.Entry, DenialReason.CredentialBlocked),
                AccessRecord.CreateDenied(credencialAndres.Id, bodega.Id, MovementType.Exit, DenialReason.CredentialBlocked),
                AccessRecord.CreateDenied(credencialPedro.Id, recepcion.Id, MovementType.Entry, DenialReason.EmployeeInactive),
                AccessRecord.CreateDenied(credencialCarlos.Id, puertaOccidental.Id, MovementType.Entry, DenialReason.AccessPointDisabled)
            ];

            await _context.AccessRecords.AddRangeAsync(records, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
