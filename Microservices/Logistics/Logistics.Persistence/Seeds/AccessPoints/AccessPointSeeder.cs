using Logistics.Domain.Entities.AccessPoints;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Seeds.AccessPoints
{
    public class AccessPointSeeder : IDataSeeder
    {
        private readonly DataContext _context;

        public AccessPointSeeder(DataContext context)
        {
            _context = context;
        }

        public int Order => 1;

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.AccessPoints.AnyAsync(cancellationToken))
            {
                return;
            }

            List<AccessPoint> accessPoints =
            [
                new AccessPoint("Recepción Principal", AccessPointType.Reception, "Ingreso peatonal de la sede principal."),
                new AccessPoint("Parqueadero Norte", AccessPointType.Parking, "Acceso vehicular y peatonal del parqueadero norte."),
                new AccessPoint("Bodega Central", AccessPointType.Warehouse, "Acceso a la bodega central de mercancías."),
                new AccessPoint("Zona de Carga", AccessPointType.Warehouse, "Andenes de cargue y descargue de vehículos."),
                new AccessPoint("Área Restringida - Servidores", AccessPointType.RestrictedArea, "Cuarto de servidores con acceso restringido.", false),
                new AccessPoint("Puerta Occidental", AccessPointType.Reception, "Acceso peatonal secundario (fuera de servicio).", false)
            ];

            await _context.AccessPoints.AddRangeAsync(accessPoints, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
