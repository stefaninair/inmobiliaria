using Inmobiliaria.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Inmobiliaria.Data
{
    public static class DbInitializer
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Verificar si ya existen usuarios
            if (context.Usuarios.Any())
            {
                return; // Ya se han inicializado los datos
            }

            // Crear usuarios de prueba
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Email = "admin@demo.com",
                    ClaveHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Rol = Rol.Administrador,
                    Nombre = "Administrador Demo"
                },
                new Usuario
                {
                    Email = "empleado@demo.com",
                    ClaveHash = BCrypt.Net.BCrypt.HashPassword("Empleado123!"),
                    Rol = Rol.Empleado,
                    Nombre = "Empleado Demo"
                }
            };

            context.Usuarios.AddRange(usuarios);
            await context.SaveChangesAsync();

            // Crear propietarios de prueba
            var propietarios = new List<Propietario>
            {
                new Propietario { Nombre = "Juan", Apellido = "Pérez", Dni = "12345678", Telefono = "1234567890", Email = "juan.perez@email.com" },
                new Propietario { Nombre = "María", Apellido = "González", Dni = "87654321", Telefono = "0987654321", Email = "maria.gonzalez@email.com" },
                new Propietario { Nombre = "Carlos", Apellido = "López", Dni = "11223344", Telefono = "1122334455", Email = "carlos.lopez@email.com" },
                new Propietario { Nombre = "Ana", Apellido = "Martín", Dni = "44332211", Telefono = "4433221100", Email = "ana.martin@email.com" }
            };

            context.Propietarios.AddRange(propietarios);
            await context.SaveChangesAsync();

            // Crear inquilinos de prueba
            var inquilinos = new List<Inquilino>
            {
                new Inquilino { Nombre = "Pedro", Apellido = "Rodríguez", Dni = "55667788", Telefono = "5566778899", Email = "pedro.rodriguez@email.com" },
                new Inquilino { Nombre = "Laura", Apellido = "Fernández", Dni = "99887766", Telefono = "9988776655", Email = "laura.fernandez@email.com" },
                new Inquilino { Nombre = "Miguel", Apellido = "Sánchez", Dni = "33445566", Telefono = "3344556677", Email = "miguel.sanchez@email.com" },
                new Inquilino { Nombre = "Sofía", Apellido = "Torres", Dni = "66778899", Telefono = "6677889900", Email = "sofia.torres@email.com" }
            };

            context.Inquilinos.AddRange(inquilinos);
            await context.SaveChangesAsync();

            // Crear tipos de inmueble
            var tiposInmueble = new List<TipoInmueble>
            {
                new TipoInmueble { Nombre = "Casa" },
                new TipoInmueble { Nombre = "Departamento" },
                new TipoInmueble { Nombre = "Local Comercial" },
                new TipoInmueble { Nombre = "Oficina" }
            };

            context.TiposInmueble.AddRange(tiposInmueble);
            await context.SaveChangesAsync();

            // Crear inmuebles de prueba
            var inmuebles = new List<Inmueble>
            {
                new Inmueble
                {
                    Direccion = "Av. Corrientes 1234, CABA",
                    Uso = "Residencial",
                    Ambientes = 3,
                    Superficie = 85.5m,
                    Precio = 150000m,
                    Disponible = true,
                    PropietarioId = propietarios[0].Id,
                    TipoInmuebleId = tiposInmueble[1].Id, // Departamento
                    Observaciones = "Departamento en excelente estado, con balcón"
                },
                new Inmueble
                {
                    Direccion = "Calle San Martín 567, CABA",
                    Uso = "Residencial",
                    Ambientes = 4,
                    Superficie = 120.0m,
                    Precio = 200000m,
                    Disponible = true,
                    PropietarioId = propietarios[1].Id,
                    TipoInmuebleId = tiposInmueble[0].Id, // Casa
                    Observaciones = "Casa con jardín, ideal para familia"
                },
                new Inmueble
                {
                    Direccion = "Av. Santa Fe 890, CABA",
                    Uso = "Comercial",
                    Ambientes = 2,
                    Superficie = 60.0m,
                    Precio = 80000m,
                    Disponible = true,
                    PropietarioId = propietarios[2].Id,
                    TipoInmuebleId = tiposInmueble[2].Id, // Local Comercial
                    Observaciones = "Local comercial en zona comercial"
                },
                new Inmueble
                {
                    Direccion = "Calle Florida 234, CABA",
                    Uso = "Comercial",
                    Ambientes = 1,
                    Superficie = 45.0m,
                    Precio = 120000m,
                    Disponible = true,
                    PropietarioId = propietarios[3].Id,
                    TipoInmuebleId = tiposInmueble[3].Id, // Oficina
                    Observaciones = "Oficina en zona céntrica"
                }
            };

            context.Inmuebles.AddRange(inmuebles);
            await context.SaveChangesAsync();

            // Crear contratos de prueba
            var adminUser = usuarios[0];
            var contratos = new List<Contrato>
            {
                new Contrato
                {
                    InmuebleId = inmuebles[0].Id,
                    InquilinoId = inquilinos[0].Id,
                    MontoMensual = 150000m,
                    FechaInicio = DateTime.Now.AddMonths(-6),
                    FechaFin = DateTime.Now.AddMonths(6),
                    CreadoPorUserId = adminUser.Id,
                    CreadoEn = DateTime.Now.AddMonths(-6)
                },
                new Contrato
                {
                    InmuebleId = inmuebles[1].Id,
                    InquilinoId = inquilinos[1].Id,
                    MontoMensual = 200000m,
                    FechaInicio = DateTime.Now.AddMonths(-3),
                    FechaFin = DateTime.Now.AddMonths(9),
                    CreadoPorUserId = adminUser.Id,
                    CreadoEn = DateTime.Now.AddMonths(-3)
                },
                new Contrato
                {
                    InmuebleId = inmuebles[2].Id,
                    InquilinoId = inquilinos[2].Id,
                    MontoMensual = 80000m,
                    FechaInicio = DateTime.Now.AddMonths(-1),
                    FechaFin = DateTime.Now.AddMonths(11),
                    CreadoPorUserId = adminUser.Id,
                    CreadoEn = DateTime.Now.AddMonths(-1)
                }
            };

            context.Contratos.AddRange(contratos);
            await context.SaveChangesAsync();

            // Crear pagos de prueba
            var pagos = new List<Pago>();
            
            // Pagos para el primer contrato (6 meses atrás)
            for (int i = 0; i < 6; i++)
            {
                pagos.Add(new Pago
                {
                    ContratoId = contratos[0].Id,
                    Monto = 150000m,
                    FechaPago = DateTime.Now.AddMonths(-6 + i).AddDays(5),
                    Periodo = DateTime.Now.AddMonths(-6 + i).ToString("yyyy-MM"),
                    CreadoPorUserId = adminUser.Id,
                    CreadoEn = DateTime.Now.AddMonths(-6 + i).AddDays(5)
                });
            }

            // Pagos para el segundo contrato (3 meses atrás)
            for (int i = 0; i < 3; i++)
            {
                pagos.Add(new Pago
                {
                    ContratoId = contratos[1].Id,
                    Monto = 200000m,
                    FechaPago = DateTime.Now.AddMonths(-3 + i).AddDays(10),
                    Periodo = DateTime.Now.AddMonths(-3 + i).ToString("yyyy-MM"),
                    CreadoPorUserId = adminUser.Id,
                    CreadoEn = DateTime.Now.AddMonths(-3 + i).AddDays(10)
                });
            }

            // Pagos para el tercer contrato (1 mes atrás)
            pagos.Add(new Pago
            {
                ContratoId = contratos[2].Id,
                Monto = 80000m,
                FechaPago = DateTime.Now.AddMonths(-1).AddDays(15),
                Periodo = DateTime.Now.AddMonths(-1).ToString("yyyy-MM"),
                CreadoPorUserId = adminUser.Id,
                CreadoEn = DateTime.Now.AddMonths(-1).AddDays(15)
            });

            context.Pagos.AddRange(pagos);
            await context.SaveChangesAsync();
        }
    }
}