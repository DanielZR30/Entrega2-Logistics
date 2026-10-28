# Sistema de Gestión y Control de Acceso de Empleados (SGCAE) - Seguimiento 2

Proyecto desarrollado para el **Seguimiento 2** aplicando **Clean Architecture (CA)**, principios de **Domain-Driven Design (DDD)**, arquitectura **CQRS (Command Query Responsibility Segregation)** y persistencia en **SQL Server** mediante **Entity Framework Core (EF Core 10)**.

La estructura de la solución, convenciones de nombres, separación por capas y utilidades desacopladas (`SimpleMediator`, `Pagination`, `DataBaseSeeder`, `ef.cmd`) replican la arquitectura del proyecto de referencia **Inmoby**.

---

## 🏛️ Arquitectura del Sistema

La solución está implementada en **.NET 10.0** y estructurada mediante el archivo de solución moderno `Logistics.slnx` organizado en carpetas virtuales:

```text
Entrega2-Logistics/
├── docker-compose.yml                      # Contenedor SQL Server 2022
├── .gitignore
├── README.md
└── Microservices/
    └── Logistics/
        ├── Logistics.slnx                  # Solución con carpetas de solución
        ├── ef.cmd                          # Automatización de migraciones EF Core
        │
        ├── Logistics.Domain/               # [/Core/] Núcleo de Dominio (DDD)
        │   ├── Common/ValueObjects/        # Value Object Email (validación de formato)
        │   ├── Entities/
        │   │   ├── Employees/              # Entidad Employee + enums de estado y área
        │   │   ├── Credentials/            # Entidad Credential + enum de estado
        │   │   ├── AccessPoints/           # Entidad AccessPoint + enum de tipo
        │   │   └── AccessRecords/          # Entidad AccessRecord + enums de movimiento,
        │   │                               #   resultado y motivo de rechazo
        │   └── Exceptions/                 # BussinesRuleException
        │
        ├── Logistics.Application/          # [/Core/] Casos de Uso (CQRS)
        │   ├── Contracts/                  # IUnitOfWork, IRepository<T> y repositorios
        │   ├── Utilities/                  # SimpleMediator desacoplado y Pagination
        │   ├── Exceptions/                 # MediatorException
        │   ├── UseCases/
        │   │   ├── Employees/Queries/      # Listado y detalle de empleados
        │   │   ├── Employees/Commands/     # Crear, actualizar y desactivar
        │   │   ├── Credentials/Queries/    # Credencial por código y por empleado
        │   │   ├── Credentials/Commands/   # Asignar, bloquear y revocar
        │   │   ├── AccessRecords/Queries/  # Historial de accesos con filtros
        │   │   ├── AccessRecords/Commands/ # Registro de accesos (RF-03)
        │   │   └── AccessPoints/           # Queries y Commands de puntos de acceso
        │   └── ApplicationServicesRegistry.cs
        │
        ├── Logistics.Persistence/          # [/Infrastructure/] Persistencia (EF Core)
        │   ├── Configurations/             # Fluent API (índices únicos y OwnsOne Email)
        │   ├── Extensions/                 # Paginación asíncrona para IQueryable
        │   ├── Repositories/               # Repository<T> genérico y específicos
        │   ├── UnitOfWorks/                # EfCoreUnitOfWork
        │   ├── Seeds/                      # IDataSeeder, DataBaseSeeder y semillas
        │   ├── Migrations/                 # Migraciones generadas con ef.cmd
        │   ├── DataContext.cs              # DbContext de EF Core
        │   └── PersistenceServicesRegistry.cs
        │
        ├── Logistics.Api/                  # [/Presenters/] Capa de Presentación (REST API)
        │   ├── Configuration/              # DotEnvLoader (.env)
        │   ├── Controllers/                # Employees, Credentials, Access y AccessPoints
        │   ├── Properties/launchSettings.json
        │   ├── appsettings.json            # Cadena de conexión a SQL Server
        │   ├── Logistics.Api.http          # Archivo de pruebas HTTP listo para ejecutar
        │   └── Program.cs                  # Pipeline HTTP, OpenAPI, Swagger y Scalar
        │
        └── Logistics.Tests/                # [/Tests/] Pruebas Unitarias
            ├── DomainTests.cs              # Reglas de negocio e invariantes
            ├── ApplicationTests.cs         # CQRS, Mediador y casos de uso
            ├── PersistenceTests.cs         # DataContext, Repositorios (EF InMemory)
            └── ControllerTests.cs          # Controladores y códigos de estado
```

---

## 📋 Casos de Uso Implementados (Seguimiento 2)

El sistema implementa **15 casos de uso** bajo el patrón CQRS: 6 de consulta (Queries) y 9 de modificación (Commands).

### RF-01. Gestión de empleados
- **`GET /api/employees`** — Consulta paginada con filtros opcionales `status`, `area` y `searchTerm`.
- **`GET /api/employees/{id}`** — Detalle del empleado con sus credenciales asignadas (404 si no existe).
- **`POST /api/employees`** — Registra un empleado con identificación única (400 si ya existe).
- **`PUT /api/employees/{id}`** — Actualiza nombres, apellidos, correo, cargo y área.
- **`DELETE /api/employees/{id}`** — Desactiva el vínculo laboral del empleado.

### RF-02. Gestión de credenciales
- **`GET /api/credentials/by-code/{code}`** — Consulta una credencial y su estado por su código único.
- **`GET /api/credentials/by-employee/{employeeId}`** — Credenciales de un empleado con paginación.
- **`POST /api/credentials`** — Asigna una credencial con código único a un empleado.
- **`PATCH /api/credentials/{id}/block`** — Bloquea una credencial vigente.
- **`PATCH /api/credentials/{id}/revoke`** — Revoca definitivamente una credencial.

### RF-03. Registro de entradas y salidas
- **`POST /api/access/register`** — Recibe código de credencial, punto de acceso y tipo de movimiento (Entrada/Salida). Valida las condiciones del empleado (activo), la credencial (vigente, no bloqueada ni revocada) y el punto de acceso (habilitado), y **registra tanto los accesos autorizados (200) como los rechazados (403) con su motivo**. Si la credencial o el punto no existen responde 404.

### RF-04. Consulta del historial de accesos
- **`GET /api/access/history`** — Movimientos paginados con filtros por empleado (`employeeId`), credencial (`credentialId`), punto de acceso (`accessPointId`), resultado de la validación (`result`) y rango de fechas (`dateFrom`, `dateTo`).

### RF-06. Administración de puntos de acceso
- **`GET /api/accesspoints`** — Consulta con filtros por tipo y estado de habilitación.
- **`POST /api/accesspoints`** — Registra un punto de acceso (Recepción, Parqueadero, Bodega, Área restringida).
- **`PATCH /api/accesspoints/{id}/status`** — Habilita o deshabilita un punto de acceso.

---

## 🚀 Guía de Puesta en Marcha

### Prerrequisitos
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (o una instancia local de SQL Server)

### Paso 1: Configurar las variables de entorno
Copia la plantilla y ajusta los valores correspondientes:
```bash
cp .env.example .env
```
> Por defecto el contenedor expone SQL Server en el puerto **1434** (para no chocar con otros contenedores locales). Si usas otro puerto, actualiza `MSSQL_PORT` y `DB_SERVER` en el `.env`.

### Paso 2: Levantar el contenedor de SQL Server
En la raíz del proyecto (`Entrega2-Logistics/`), ejecuta:
```bash
docker compose up -d
```
Esto iniciará un contenedor SQL Server 2022 con credenciales:
- **Usuario**: `sa`
- **Contraseña**: `Logistics@2026!`
- **Base de Datos**: `LogisticsDb`

### Paso 3: Aplicar migraciones con `ef.cmd`
Navega a la carpeta del microservicio y ejecuta el script de migraciones:
```cmd
cd Microservices\Logistics
ef.cmd update
```

### Paso 4: Iniciar la API
Ejecuta la API desde la carpeta del microservicio:
```bash
dotnet run --project Logistics.Api\Logistics.Api.csproj
```
Al arrancar, el sistema ejecutará automáticamente el semillero (`DataBaseSeeder`) que precarga:
- **6 Puntos de acceso**: Recepción Principal, Parqueadero Norte, Bodega Central, Zona de Carga, Área Restringida - Servidores y Puerta Occidental (2 deshabilitados).
- **6 Empleados**: Operarios, auxiliares, seguridad, coordinación y mantenimiento (1 inactivo).
- **6 Credenciales**: códigos `CRD-0001` a `CRD-0006` (1 bloqueada y 1 revocada).
- **10 Movimientos de acceso**: entradas y salidas autorizadas y rechazadas con distintos motivos.

---

## 🎨 Interfaces Visuales de Documentación

Una vez iniciada la API en `http://localhost:5100`:

1. **Scalar API Reference (Moderna y Elegante)**:
   - **URL**: [http://localhost:5100/](http://localhost:5100/) (o `/scalar/v1`)
   - Incluye tema *DeepSpace*, generador de snippets en cURL/C#/JS/Python, modo oscuro y consola interactiva de pruebas.

2. **Swagger UI (Clásica)**:
   - **URL**: [http://localhost:5100/swagger](http://localhost:5100/swagger)
   - Especificación OpenAPI JSON: [http://localhost:5100/swagger/v1/swagger.json](http://localhost:5100/swagger/v1/swagger.json)

---

## 🧪 Pruebas Automatizadas

La solución cuenta con **45 pruebas unitarias** que cubren el Dominio, la Capa de Aplicación, la Persistencia y los Controladores:

| Suite | Cobertura | Pruebas |
|---|---|---|
| `DomainTests` | Entidades, Value Object `Email`, reglas e invariantes | 20 |
| `ApplicationTests` | Mediador, casos de uso y registro de accesos | 10 |
| `PersistenceTests` | DataContext, repositorios y filtros (EF InMemory) | 6 |
| `ControllerTests` | Controladores y códigos de estado HTTP | 9 |

Para ejecutar todas las pruebas:
```bash
dotnet test Microservices\Logistics\Logistics.slnx
```
**Resultado**: `Correctas! - Con error: 0, Superado: 45, Omitido: 0, Total: 45`

---

## 🌿 Flujo de Ramas Git (Git Flow)

El desarrollo del proyecto se gestionó mediante fases y ramas locales:
- `main`: Rama principal de entrega final etiquetada con `v2.0.0-entrega`.
- `develop`: Rama base de integración continua.
- `feature/fase-1-setup`: Inicialización de proyectos y estructura espejo de Inmoby.
- `feature/fase-2-dominio`: Entidades DDD, Value Object `Email`, enums, reglas de negocio y pruebas.
- `feature/fase-3-aplicacion`: `SimpleMediator`, `Pagination`, DTOs, Queries y Commands CQRS.
- `feature/fase-4-persistencia`: `DataContext`, Fluent API, repositorios, migraciones y seeders.
- `feature/fase-5-api`: Controladores REST, OpenAPI, Swagger UI y Scalar API Reference.
- `feature/fase-6-cierre`: README completo y release de la entrega.
