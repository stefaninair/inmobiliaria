using Inmobiliaria.Models;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace Inmobiliaria.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DatabaseConnection dbConnection)
        {
            using var connection = dbConnection.GetConnection();
            connection.Open();

            try
            {
                // Primero crear las tablas si no existen
                CreateTables(connection);

                // Luego verificar si ya existen datos
                var countQuery = "SELECT COUNT(*) FROM Usuarios";
                using var countCommand = connection.CreateCommand();
                countCommand.CommandText = countQuery;
                var count = Convert.ToInt32(countCommand.ExecuteScalar());

                if (count > 0)
                {
                    Console.WriteLine("Base de datos ya tiene datos, saltando inicialización.");
                    return; // Ya hay datos, no inicializar
                }
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
                throw;
            }

            // Insertar tipos de inmueble
            var tiposInmueble = new List<TipoInmueble>
            {
                new TipoInmueble { Nombre = "Casa" },
                new TipoInmueble { Nombre = "Departamento" },
                new TipoInmueble { Nombre = "Local Comercial" },
                new TipoInmueble { Nombre = "Oficina" },
                new TipoInmueble { Nombre = "Terreno" }
            };

            var tiposInmuebleIds = new List<int>();
            foreach (var tipo in tiposInmueble)
            {
                // Verificar si el tipo ya existe
                var checkQuery = "SELECT Id FROM TiposInmueble WHERE Nombre = @nombre";
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = checkQuery;
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@nombre", tipo.Nombre));
                var existingId = checkCommand.ExecuteScalar();

                if (existingId != null)
                {
                    tiposInmuebleIds.Add(Convert.ToInt32(existingId));
                }
                else
                {
                    var insertQuery = "INSERT INTO TiposInmueble (Nombre) VALUES (@nombre)";
                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = insertQuery;
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@nombre", tipo.Nombre));
                    insertCommand.ExecuteNonQuery();

                    var lastIdQuery = dbConnection.GetLastInsertIdQuery();
                    using var lastIdCommand = connection.CreateCommand();
                    lastIdCommand.CommandText = lastIdQuery;
                    var lastId = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                    tiposInmuebleIds.Add(lastId);
                }
            }

            // Insertar usuarios
            var usuarios = new List<Usuario>
            {
                new Usuario { Nombre = "Admin", Apellido = "Sistema", Email = "admin@inmobiliaria.com", ClaveHash = BCrypt.Net.BCrypt.HashPassword("123456"), Rol = Rol.Administrador },
                new Usuario { Nombre = "Juan", Apellido = "Pérez", Email = "juan@inmobiliaria.com", ClaveHash = BCrypt.Net.BCrypt.HashPassword("123456"), Rol = Rol.Empleado }
            };

            var usuarioIds = new List<int>();
            foreach (var usuario in usuarios)
            {
                // Verificar si el usuario ya existe
                var checkQuery = "SELECT Id FROM Usuarios WHERE Email = @email";
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = checkQuery;
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@email", usuario.Email));
                var existingId = checkCommand.ExecuteScalar();

                if (existingId != null)
                {
                    usuarioIds.Add(Convert.ToInt32(existingId));
                }
                else
                {
                    var insertQuery = "INSERT INTO Usuarios (Nombre, Apellido, Email, Clave, Rol) VALUES (@nombre, @apellido, @email, @clave, @rol)";
                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = insertQuery;
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@nombre", usuario.Nombre));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@apellido", usuario.Apellido));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@email", usuario.Email));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@clave", usuario.ClaveHash));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@rol", usuario.Rol.ToString()));
                    insertCommand.ExecuteNonQuery();

                    var lastIdQuery = dbConnection.GetLastInsertIdQuery();
                    using var lastIdCommand = connection.CreateCommand();
                    lastIdCommand.CommandText = lastIdQuery;
                    var lastId = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                    usuarioIds.Add(lastId);
                }
            }

            // Insertar propietarios
            var propietarios = new List<Propietario>
            {
                new Propietario { Nombre = "Carlos", Apellido = "López", Dni = "12345678", Telefono = "1234567890", Email = "carlos@email.com", Clave = BCrypt.Net.BCrypt.HashPassword("123456") },
                new Propietario { Nombre = "María", Apellido = "González", Dni = "87654321", Telefono = "0987654321", Email = "maria@email.com", Clave = BCrypt.Net.BCrypt.HashPassword("123456") },
                new Propietario { Nombre = "Ana", Apellido = "Martínez", Dni = "11223344", Telefono = "1122334455", Email = "ana@email.com", Clave = BCrypt.Net.BCrypt.HashPassword("123456") }
            };

            var propietarioIds = new List<int>();
            foreach (var propietario in propietarios)
            {
                // Verificar si el propietario ya existe
                var checkQuery = "SELECT Id FROM Propietarios WHERE Dni = @dni";
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = checkQuery;
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@dni", propietario.Dni));
                var existingId = checkCommand.ExecuteScalar();

                if (existingId != null)
                {
                    propietarioIds.Add(Convert.ToInt32(existingId));
                }
                else
                {
                    var insertQuery = "INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email, Clave) VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave)";
                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = insertQuery;
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@nombre", propietario.Nombre));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@apellido", propietario.Apellido));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@dni", propietario.Dni));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@telefono", propietario.Telefono));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@email", propietario.Email));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@clave", propietario.Clave));
                    insertCommand.ExecuteNonQuery();

                    var lastIdQuery = dbConnection.GetLastInsertIdQuery();
                    using var lastIdCommand = connection.CreateCommand();
                    lastIdCommand.CommandText = lastIdQuery;
                    var lastId = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                    propietarioIds.Add(lastId);
                }
            }

            // Insertar inquilinos
            var inquilinos = new List<Inquilino>
            {
                new Inquilino { Nombre = "Luis", Apellido = "Rodríguez", Dni = "55667788", Telefono = "5566778899", Email = "luis@email.com" },
                new Inquilino { Nombre = "Sofia", Apellido = "Hernández", Dni = "99887766", Telefono = "9988776655", Email = "sofia@email.com" },
                new Inquilino { Nombre = "Pedro", Apellido = "García", Dni = "44332211", Telefono = "4433221100", Email = "pedro@email.com" }
            };

            var inquilinoIds = new List<int>();
            foreach (var inquilino in inquilinos)
            {
                // Verificar si el inquilino ya existe
                var checkQuery = "SELECT Id FROM Inquilinos WHERE Dni = @dni";
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = checkQuery;
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@dni", inquilino.Dni));
                var existingId = checkCommand.ExecuteScalar();

                if (existingId != null)
                {
                    inquilinoIds.Add(Convert.ToInt32(existingId));
                }
                else
                {
                    var insertQuery = "INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email) VALUES (@nombre, @apellido, @dni, @telefono, @email)";
                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = insertQuery;
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@nombre", inquilino.Nombre));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@apellido", inquilino.Apellido));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@dni", inquilino.Dni));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@telefono", inquilino.Telefono));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@email", inquilino.Email));
                    insertCommand.ExecuteNonQuery();

                    var lastIdQuery = dbConnection.GetLastInsertIdQuery();
                    using var lastIdCommand = connection.CreateCommand();
                    lastIdCommand.CommandText = lastIdQuery;
                    var lastId = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                    inquilinoIds.Add(lastId);
                }
            }

            // Insertar inmuebles
            var inmuebles = new List<Inmueble>
            {
                new Inmueble { Direccion = "Av. Corrientes 1234, CABA", Uso = "Residencial", Ambientes = 3, Superficie = 85.50m, Precio = 150000.00m, Disponible = true, PropietarioId = propietarioIds[0], TipoInmuebleId = tiposInmuebleIds[0], Observaciones = "Casa con patio" },
                new Inmueble { Direccion = "Av. Santa Fe 5678, CABA", Uso = "Residencial", Ambientes = 2, Superficie = 65.00m, Precio = 120000.00m, Disponible = true, PropietarioId = propietarioIds[1], TipoInmuebleId = tiposInmuebleIds[1], Observaciones = "Departamento con balcón" },
                new Inmueble { Direccion = "Av. Córdoba 9012, CABA", Uso = "Comercial", Ambientes = 1, Superficie = 45.00m, Precio = 80000.00m, Disponible = true, PropietarioId = propietarioIds[2], TipoInmuebleId = tiposInmuebleIds[2], Observaciones = "Local comercial en planta baja" },
                new Inmueble { Direccion = "Av. Rivadavia 3456, CABA", Uso = "Residencial", Ambientes = 4, Superficie = 120.00m, Precio = 200000.00m, Disponible = false, PropietarioId = propietarioIds[0], TipoInmuebleId = tiposInmuebleIds[0], Observaciones = "Casa con garaje" },
                new Inmueble { Direccion = "Av. Callao 7890, CABA", Uso = "Residencial", Ambientes = 1, Superficie = 35.00m, Precio = 90000.00m, Disponible = true, PropietarioId = propietarioIds[1], TipoInmuebleId = tiposInmuebleIds[1], Observaciones = "Monoambiente" }
            };

            var inmuebleIds = new List<int>();
            foreach (var inmueble in inmuebles)
            {
                // Verificar si el inmueble ya existe
                var checkQuery = "SELECT Id FROM Inmuebles WHERE Direccion = @direccion";
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = checkQuery;
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@direccion", inmueble.Direccion));
                var existingId = checkCommand.ExecuteScalar();

                if (existingId != null)
                {
                    inmuebleIds.Add(Convert.ToInt32(existingId));
                }
                else
                {
                    var insertQuery = "INSERT INTO Inmuebles (Direccion, Uso, Ambientes, Superficie, Precio, Disponible, PropietarioId, TipoInmuebleId, Observaciones) VALUES (@direccion, @uso, @ambientes, @superficie, @precio, @disponible, @propietarioId, @tipoInmuebleId, @observaciones)";
                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = insertQuery;
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@direccion", inmueble.Direccion));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@uso", inmueble.Uso ?? (object)DBNull.Value));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@ambientes", inmueble.Ambientes));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@superficie", inmueble.Superficie));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@precio", inmueble.Precio));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@disponible", inmueble.Disponible));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@propietarioId", inmueble.PropietarioId));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@tipoInmuebleId", inmueble.TipoInmuebleId));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@observaciones", inmueble.Observaciones ?? (object)DBNull.Value));
                    insertCommand.ExecuteNonQuery();

                    var lastIdQuery = dbConnection.GetLastInsertIdQuery();
                    using var lastIdCommand = connection.CreateCommand();
                    lastIdCommand.CommandText = lastIdQuery;
                    var lastId = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                    inmuebleIds.Add(lastId);
                }
            }

            // Insertar contratos
            var contratos = new List<Contrato>
            {
                new Contrato { InmuebleId = inmuebleIds[3], InquilinoId = inquilinoIds[0], MontoMensual = 200000.00m, FechaInicio = new DateTime(2024, 1, 1), FechaFin = new DateTime(2024, 12, 31), CreadoPorUserId = usuarioIds[0] },
                new Contrato { InmuebleId = inmuebleIds[0], InquilinoId = inquilinoIds[1], MontoMensual = 150000.00m, FechaInicio = new DateTime(2024, 2, 1), FechaFin = new DateTime(2025, 1, 31), CreadoPorUserId = usuarioIds[0] },
                new Contrato { InmuebleId = inmuebleIds[1], InquilinoId = inquilinoIds[2], MontoMensual = 120000.00m, FechaInicio = new DateTime(2024, 3, 1), FechaFin = new DateTime(2025, 2, 28), CreadoPorUserId = usuarioIds[0] }
            };

            var contratoIds = new List<int>();
            foreach (var contrato in contratos)
            {
                // Verificar si el contrato ya existe
                var checkQuery = "SELECT Id FROM Contratos WHERE InmuebleId = @inmuebleId AND InquilinoId = @inquilinoId AND FechaInicio = @fechaInicio";
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = checkQuery;
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@inmuebleId", contrato.InmuebleId));
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@inquilinoId", contrato.InquilinoId));
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@fechaInicio", contrato.FechaInicio));
                var existingId = checkCommand.ExecuteScalar();

                if (existingId != null)
                {
                    contratoIds.Add(Convert.ToInt32(existingId));
                }
                else
                {
                    var insertQuery = "INSERT INTO Contratos (InmuebleId, InquilinoId, MontoMensual, FechaInicio, FechaFin, CreadoPorUserId) VALUES (@inmuebleId, @inquilinoId, @montoMensual, @fechaInicio, @fechaFin, @creadoPorUserId)";
                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = insertQuery;
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@inmuebleId", contrato.InmuebleId));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@inquilinoId", contrato.InquilinoId));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@montoMensual", contrato.MontoMensual));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@fechaInicio", contrato.FechaInicio));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@fechaFin", contrato.FechaFin));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@creadoPorUserId", contrato.CreadoPorUserId));
                    insertCommand.ExecuteNonQuery();

                    var lastIdQuery = dbConnection.GetLastInsertIdQuery();
                    using var lastIdCommand = connection.CreateCommand();
                    lastIdCommand.CommandText = lastIdQuery;
                    var lastId = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                    contratoIds.Add(lastId);
                }
            }

            // Insertar pagos
            var pagos = new List<Pago>
            {
                new Pago { ContratoId = contratoIds[0], Monto = 200000.00m, FechaPago = new DateTime(2024, 1, 5), Periodo = "2024-01", CreadoPorUserId = usuarioIds[0] },
                new Pago { ContratoId = contratoIds[0], Monto = 200000.00m, FechaPago = new DateTime(2024, 2, 5), Periodo = "2024-02", CreadoPorUserId = usuarioIds[0] },
                new Pago { ContratoId = contratoIds[0], Monto = 200000.00m, FechaPago = new DateTime(2024, 3, 5), Periodo = "2024-03", CreadoPorUserId = usuarioIds[0] },
                new Pago { ContratoId = contratoIds[1], Monto = 150000.00m, FechaPago = new DateTime(2024, 2, 10), Periodo = "2024-02", CreadoPorUserId = usuarioIds[0] },
                new Pago { ContratoId = contratoIds[1], Monto = 150000.00m, FechaPago = new DateTime(2024, 3, 10), Periodo = "2024-03", CreadoPorUserId = usuarioIds[0] },
                new Pago { ContratoId = contratoIds[2], Monto = 120000.00m, FechaPago = new DateTime(2024, 3, 15), Periodo = "2024-03", CreadoPorUserId = usuarioIds[0] }
            };

            foreach (var pago in pagos)
            {
                // Verificar si el pago ya existe
                var checkQuery = "SELECT Id FROM Pagos WHERE ContratoId = @contratoId AND Periodo = @periodo AND FechaPago = @fechaPago";
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = checkQuery;
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@contratoId", pago.ContratoId));
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@periodo", pago.Periodo));
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@fechaPago", pago.FechaPago));
                var existingId = checkCommand.ExecuteScalar();

                if (existingId == null)
                {
                    var insertQuery = "INSERT INTO Pagos (ContratoId, Monto, FechaPago, Periodo, CreadoPorUserId) VALUES (@contratoId, @monto, @fechaPago, @periodo, @creadoPorUserId)";
                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = insertQuery;
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@contratoId", pago.ContratoId));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@monto", pago.Monto));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@fechaPago", pago.FechaPago));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@periodo", pago.Periodo));
                    insertCommand.Parameters.Add(CreateParameter(insertCommand, "@creadoPorUserId", pago.CreadoPorUserId));
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        private static IDbDataParameter CreateParameter(IDbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        private static void CreateTables(IDbConnection connection)
        {
            try
            {
                Console.WriteLine("Creando tablas de la base de datos...");
                var createTablesScript = @"
                -- Tabla de Tipos de Inmueble
                CREATE TABLE IF NOT EXISTS TiposInmueble (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL UNIQUE
                );

                -- Tabla de Usuarios
                CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    Clave TEXT NOT NULL,
                    AvatarPath TEXT,
                    Rol TEXT NOT NULL DEFAULT 'Empleado' CHECK (Rol IN ('Administrador', 'Empleado')),
                    Activo INTEGER NOT NULL DEFAULT 1,
                    CreadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ActualizadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                -- Tabla de Propietarios
                CREATE TABLE IF NOT EXISTS Propietarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Dni TEXT NOT NULL UNIQUE,
                    Telefono TEXT,
                    Email TEXT,
                    Clave TEXT NOT NULL,
                    Activo INTEGER NOT NULL DEFAULT 1,
                    CreadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ActualizadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                -- Tabla de Inquilinos
                CREATE TABLE IF NOT EXISTS Inquilinos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Dni TEXT NOT NULL UNIQUE,
                    Telefono TEXT,
                    Email TEXT,
                    Activo INTEGER NOT NULL DEFAULT 1,
                    CreadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ActualizadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                -- Tabla de Inmuebles
                CREATE TABLE IF NOT EXISTS Inmuebles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Direccion TEXT NOT NULL,
                    Uso TEXT NOT NULL,
                    Ambientes INTEGER NOT NULL,
                    Superficie REAL NOT NULL,
                    Precio REAL NOT NULL,
                    Disponible INTEGER NOT NULL DEFAULT 1,
                    PropietarioId INTEGER NOT NULL,
                    TipoInmuebleId INTEGER NOT NULL,
                    Observaciones TEXT,
                    Portada TEXT,
                    CreadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ActualizadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (PropietarioId) REFERENCES Propietarios(Id) ON DELETE CASCADE,
                    FOREIGN KEY (TipoInmuebleId) REFERENCES TiposInmueble(Id) ON DELETE CASCADE
                );

                -- Tabla de Imagenes
                CREATE TABLE IF NOT EXISTS Imagenes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InmuebleId INTEGER NOT NULL,
                    Url TEXT NOT NULL,
                    CreadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (InmuebleId) REFERENCES Inmuebles(Id) ON DELETE CASCADE
                );

                -- Tabla de Contratos
                CREATE TABLE IF NOT EXISTS Contratos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InmuebleId INTEGER NOT NULL,
                    InquilinoId INTEGER NOT NULL,
                    MontoMensual REAL NOT NULL,
                    FechaInicio DATE NOT NULL,
                    FechaFin DATE NOT NULL,
                    FechaTerminacionAnticipada DATE NULL,
                    Multa REAL NULL,
                    MotivoTerminacion TEXT NULL,
                    CreadoPorUserId INTEGER NOT NULL,
                    CreadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ActualizadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (InmuebleId) REFERENCES Inmuebles(Id) ON DELETE CASCADE,
                    FOREIGN KEY (InquilinoId) REFERENCES Inquilinos(Id) ON DELETE CASCADE,
                    FOREIGN KEY (CreadoPorUserId) REFERENCES Usuarios(Id) ON DELETE CASCADE
                );

                -- Tabla de Pagos
                CREATE TABLE IF NOT EXISTS Pagos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ContratoId INTEGER NOT NULL,
                    Monto REAL NOT NULL,
                    FechaPago DATE NOT NULL,
                    Periodo TEXT NOT NULL,
                    Observaciones TEXT,
                    CreadoPorUserId INTEGER NOT NULL,
                    CreadoEn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    AnuladoPorUserId INTEGER NULL,
                    AnuladoEn DATETIME NULL,
                    MotivoAnulacion TEXT,
                    Eliminado INTEGER NOT NULL DEFAULT 0,
                    EliminadoPorUserId INTEGER NULL,
                    EliminadoEn DATETIME NULL,
                    FOREIGN KEY (ContratoId) REFERENCES Contratos(Id) ON DELETE CASCADE,
                    FOREIGN KEY (CreadoPorUserId) REFERENCES Usuarios(Id) ON DELETE CASCADE,
                    FOREIGN KEY (AnuladoPorUserId) REFERENCES Usuarios(Id) ON DELETE SET NULL,
                    FOREIGN KEY (EliminadoPorUserId) REFERENCES Usuarios(Id) ON DELETE SET NULL
                );

                -- Migración: Agregar columnas faltantes a la tabla Contratos
                -- Nota: SQLite no soporta ALTER TABLE condicional, por lo que usamos try-catch
                -- Las columnas se agregarán en el código C# usando try-catch

                -- Crear índices para mejorar el rendimiento
                CREATE INDEX IF NOT EXISTS idx_propietarios_dni ON Propietarios(Dni);
                CREATE INDEX IF NOT EXISTS idx_inquilinos_dni ON Inquilinos(Dni);
                CREATE INDEX IF NOT EXISTS idx_inmuebles_propietario ON Inmuebles(PropietarioId);
                CREATE INDEX IF NOT EXISTS idx_inmuebles_tipo ON Inmuebles(TipoInmuebleId);
                CREATE INDEX IF NOT EXISTS idx_inmuebles_disponible ON Inmuebles(Disponible);
                CREATE INDEX IF NOT EXISTS idx_contratos_inmueble ON Contratos(InmuebleId);
                CREATE INDEX IF NOT EXISTS idx_contratos_inquilino ON Contratos(InquilinoId);
                CREATE INDEX IF NOT EXISTS idx_pagos_contrato ON Pagos(ContratoId);
                CREATE INDEX IF NOT EXISTS idx_pagos_periodo ON Pagos(Periodo);
                CREATE INDEX IF NOT EXISTS idx_pagos_fecha ON Pagos(FechaPago);
            ";

            var commands = createTablesScript.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var command in commands)
            {
                if (!string.IsNullOrWhiteSpace(command.Trim()))
                {
                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = command.Trim();
                    cmd.ExecuteNonQuery();
                }
            }

            // Migración: Agregar columnas faltantes a la tabla Contratos
            try
            {
                using var alterCmd1 = connection.CreateCommand();
                alterCmd1.CommandText = "ALTER TABLE Contratos ADD COLUMN FechaTerminacionAnticipada DATE NULL";
                alterCmd1.ExecuteNonQuery();
            }
            catch (Exception)
            {
                // La columna ya existe, continuar
            }

            try
            {
                using var alterCmd2 = connection.CreateCommand();
                alterCmd2.CommandText = "ALTER TABLE Contratos ADD COLUMN Multa REAL NULL";
                alterCmd2.ExecuteNonQuery();
            }
            catch (Exception)
            {
                // La columna ya existe, continuar
            }

            try
            {
                using var alterCmd3 = connection.CreateCommand();
                alterCmd3.CommandText = "ALTER TABLE Contratos ADD COLUMN MotivoTerminacion TEXT NULL";
                alterCmd3.ExecuteNonQuery();
            }
            catch (Exception)
            {
                // La columna ya existe, continuar
            }

            // Migración: Agregar columna Portada a la tabla Inmuebles
            try
            {
                using var alterCmd4 = connection.CreateCommand();
                alterCmd4.CommandText = "ALTER TABLE Inmuebles ADD COLUMN Portada TEXT NULL";
                alterCmd4.ExecuteNonQuery();
            }
            catch (Exception)
            {
                // La columna ya existe, continuar
            }
            Console.WriteLine("Tablas creadas exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear las tablas: {ex.Message}");
                throw;
            }
        }
    }
}