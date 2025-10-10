using Inmobiliaria.Data;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioContrato : BaseRepository
    {
        public RepositorioContrato(DatabaseConnection dbConnection) : base(dbConnection)
        {
        }

        public List<Contrato> ObtenerTodos()
        {
            var contratos = new List<Contrato>();
            var query = @"
                SELECT c.Id, c.InmuebleId, c.InquilinoId, c.MontoMensual, c.FechaInicio, c.FechaFin, 
                       c.FechaTerminacionAnticipada, c.Multa, c.MotivoTerminacion,
                       c.CreadoPorUserId, c.CreadoEn,
                       i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible,
                       p.Nombre as PropietarioNombre, p.Apellido as PropietarioApellido, p.Dni as PropietarioDni,
                       inq.Nombre as InquilinoNombre, inq.Apellido as InquilinoApellido, inq.Dni as InquilinoDni
                FROM Contratos c
                LEFT JOIN Inmuebles i ON c.InmuebleId = i.Id
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN Inquilinos inq ON c.InquilinoId = inq.Id
                ORDER BY c.Id";

            using var reader = ExecuteReader(query);
            while (reader.Read())
            {
                contratos.Add(MapFromReader(reader));
            }

            return contratos;
        }

        public Contrato? ObtenerPorId(int id)
        {
            var query = @"
                SELECT c.Id, c.InmuebleId, c.InquilinoId, c.MontoMensual, c.FechaInicio, c.FechaFin, 
                       c.FechaTerminacionAnticipada, c.Multa, c.MotivoTerminacion,
                       c.CreadoPorUserId, c.CreadoEn,
                       i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible,
                       p.Nombre as PropietarioNombre, p.Apellido as PropietarioApellido, p.Dni as PropietarioDni,
                       inq.Nombre as InquilinoNombre, inq.Apellido as InquilinoApellido, inq.Dni as InquilinoDni
                FROM Contratos c
                LEFT JOIN Inmuebles i ON c.InmuebleId = i.Id
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN Inquilinos inq ON c.InquilinoId = inq.Id
                WHERE c.Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            using var reader = ExecuteReader(query, parameters);
            if (reader.Read())
            {
                return MapFromReader(reader);
            }

            return null;
        }

        public int Alta(Contrato c)
        {
            var query = "INSERT INTO Contratos (InmuebleId, InquilinoId, MontoMensual, FechaInicio, FechaFin, CreadoPorUserId) VALUES (@inmuebleId, @inquilinoId, @montoMensual, @fechaInicio, @fechaFin, @creadoPorUserId)";
            var parameters = new Dictionary<string, object>
            {
                { "@inmuebleId", c.InmuebleId },
                { "@inquilinoId", c.InquilinoId },
                { "@montoMensual", c.MontoMensual },
                { "@fechaInicio", c.FechaInicio },
                { "@fechaFin", c.FechaFin },
                { "@creadoPorUserId", c.CreadoPorUserId }
            };

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = CreateCommand(query, connection, parameters);
            command.ExecuteNonQuery();
            
            return GetLastInsertId(connection);
        }

        public int Modificacion(Contrato c)
        {
            var query = "UPDATE Contratos SET InmuebleId = @inmuebleId, InquilinoId = @inquilinoId, MontoMensual = @montoMensual, FechaInicio = @fechaInicio, FechaFin = @fechaFin WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", c.Id },
                { "@inmuebleId", c.InmuebleId },
                { "@inquilinoId", c.InquilinoId },
                { "@montoMensual", c.MontoMensual },
                { "@fechaInicio", c.FechaInicio },
                { "@fechaFin", c.FechaFin }
            };

            return ExecuteNonQuery(query, parameters);
        }

        public int Baja(int id)
        {
            var query = "DELETE FROM Contratos WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            return ExecuteNonQuery(query, parameters);
        }

        public List<Contrato> ObtenerContratosActivosPorInmueble(int inmuebleId)
        {
            var contratos = new List<Contrato>();
            var query = @"
                SELECT c.Id, c.InmuebleId, c.InquilinoId, c.MontoMensual, c.FechaInicio, c.FechaFin, 
                       c.CreadoPorUserId, c.CreadoEn,
                       i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible,
                       p.Nombre as PropietarioNombre, p.Apellido as PropietarioApellido, p.Dni as PropietarioDni,
                       inq.Nombre as InquilinoNombre, inq.Apellido as InquilinoApellido, inq.Dni as InquilinoDni
                FROM Contratos c
                LEFT JOIN Inmuebles i ON c.InmuebleId = i.Id
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN Inquilinos inq ON c.InquilinoId = inq.Id
                WHERE c.InmuebleId = @inmuebleId AND c.FechaFin >= date('now')
                ORDER BY c.FechaInicio DESC";
            var parameters = new Dictionary<string, object> { { "@inmuebleId", inmuebleId } };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                contratos.Add(MapFromReader(reader));
            }

            return contratos;
        }

        public List<Contrato> ObtenerContratosPorInquilino(int inquilinoId)
        {
            var contratos = new List<Contrato>();
            var query = @"
                SELECT c.Id, c.InmuebleId, c.InquilinoId, c.MontoMensual, c.FechaInicio, c.FechaFin, 
                       c.CreadoPorUserId, c.CreadoEn,
                       i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible,
                       p.Nombre as PropietarioNombre, p.Apellido as PropietarioApellido, p.Dni as PropietarioDni,
                       inq.Nombre as InquilinoNombre, inq.Apellido as InquilinoApellido, inq.Dni as InquilinoDni
                FROM Contratos c
                LEFT JOIN Inmuebles i ON c.InmuebleId = i.Id
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN Inquilinos inq ON c.InquilinoId = inq.Id
                WHERE c.InquilinoId = @inquilinoId
                ORDER BY c.FechaInicio DESC";
            var parameters = new Dictionary<string, object> { { "@inquilinoId", inquilinoId } };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                contratos.Add(MapFromReader(reader));
            }

            return contratos;
        }

        public List<Contrato> ObtenerPaginados(int pagina, int tamanoPagina)
        {
            var contratos = new List<Contrato>();
            var offset = (pagina - 1) * tamanoPagina;
            var query = @"
                SELECT c.Id, c.InmuebleId, c.InquilinoId, c.MontoMensual, c.FechaInicio, c.FechaFin, 
                       c.FechaTerminacionAnticipada, c.Multa, c.MotivoTerminacion,
                       c.CreadoPorUserId, c.CreadoEn,
                       i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible,
                       p.Nombre as PropietarioNombre, p.Apellido as PropietarioApellido, p.Dni as PropietarioDni,
                       inq.Nombre as InquilinoNombre, inq.Apellido as InquilinoApellido, inq.Dni as InquilinoDni
                FROM Contratos c
                LEFT JOIN Inmuebles i ON c.InmuebleId = i.Id
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN Inquilinos inq ON c.InquilinoId = inq.Id
                ORDER BY c.Id LIMIT @offset, @tamano";
            var parameters = new Dictionary<string, object>
            {
                { "@offset", offset },
                { "@tamano", tamanoPagina }
            };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                contratos.Add(MapFromReader(reader));
            }

            return contratos;
        }

        public int Contar()
        {
            var query = "SELECT COUNT(*) FROM Contratos";
            var result = ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public int RenovarContrato(int contratoId, DateTime nuevaFechaFin, decimal? nuevoMontoMensual = null)
        {
            var query = @"
                UPDATE Contratos 
                SET FechaFin = @nuevaFechaFin, 
                    MontoMensual = COALESCE(@nuevoMontoMensual, MontoMensual)
                WHERE Id = @contratoId";
            var parameters = new Dictionary<string, object>
            {
                { "@contratoId", contratoId },
                { "@nuevaFechaFin", nuevaFechaFin }
            };

            if (nuevoMontoMensual.HasValue)
            {
                parameters.Add("@nuevoMontoMensual", nuevoMontoMensual.Value);
            }
            else
            {
                parameters.Add("@nuevoMontoMensual", DBNull.Value);
            }

            return ExecuteNonQuery(query, parameters);
        }

        public int TerminarContrato(int contratoId, DateTime fechaTerminacion, decimal? multa = null, string? motivo = null)
        {
            Console.WriteLine($"Repositorio - Terminando contrato {contratoId}:");
            Console.WriteLine($"Fecha: {fechaTerminacion}, Multa: {multa}, Motivo: {motivo}");
            
            var query = @"
                UPDATE Contratos 
                SET FechaTerminacionAnticipada = @fechaTerminacion,
                    Multa = @multa,
                    MotivoTerminacion = @motivo
                WHERE Id = @contratoId";
            var parameters = new Dictionary<string, object>
            {
                { "@contratoId", contratoId },
                { "@fechaTerminacion", fechaTerminacion },
                { "@multa", multa ?? (object)DBNull.Value },
                { "@motivo", motivo ?? (object)DBNull.Value }
            };

            Console.WriteLine($"Query: {query}");
            foreach (var param in parameters)
            {
                Console.WriteLine($"  {param.Key}: {param.Value}");
            }

            var result = ExecuteNonQuery(query, parameters);
            Console.WriteLine($"Resultado: {result} filas afectadas");
            return result;
        }

        private Contrato MapFromReader(IDataReader reader)
        {
            return new Contrato
            {
                Id = reader.GetInt32("Id"),
                InmuebleId = reader.GetInt32("InmuebleId"),
                InquilinoId = reader.GetInt32("InquilinoId"),
                MontoMensual = reader.GetDecimal("MontoMensual"),
                FechaInicio = reader.GetDateTime("FechaInicio"),
                FechaFin = reader.GetDateTime("FechaFin"),
                FechaTerminacionAnticipada = reader.IsDBNull("FechaTerminacionAnticipada") ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                Multa = reader.IsDBNull("Multa") ? null : reader.GetDecimal("Multa"),
                MotivoTerminacion = reader.IsDBNull("MotivoTerminacion") ? null : reader.GetString("MotivoTerminacion"),
                CreadoPorUserId = reader.GetInt32("CreadoPorUserId"),
                CreadoEn = reader.GetDateTime("CreadoEn"),
                Inmueble = new Inmueble
                {
                    Id = reader.GetInt32("InmuebleId"),
                    Direccion = reader.GetString("Direccion"),
                    Uso = reader.GetString("Uso"),
                    Ambientes = reader.GetInt32("Ambientes"),
                    Superficie = reader.GetDecimal("Superficie"),
                    Precio = reader.GetDecimal("Precio"),
                    Disponible = reader.GetBoolean("Disponible"),
                    Propietario = reader.IsDBNull("PropietarioNombre") ? null : new Propietario
                    {
                        Nombre = reader.GetString("PropietarioNombre"),
                        Apellido = reader.GetString("PropietarioApellido"),
                        Dni = reader.GetString("PropietarioDni")
                    }
                },
                Inquilino = reader.IsDBNull("InquilinoNombre") ? null : new Inquilino
                {
                    Id = reader.GetInt32("InquilinoId"),
                    Nombre = reader.GetString("InquilinoNombre"),
                    Apellido = reader.GetString("InquilinoApellido"),
                    Dni = reader.GetString("InquilinoDni")
                }
            };
        }

        public List<Pago> ObtenerPagosPorContrato(int contratoId)
        {
            var pagos = new List<Pago>();
            var query = @"
                SELECT p.Id, p.ContratoId, p.Monto, p.FechaPago, p.Periodo, p.Observaciones,
                       p.CreadoPorUserId, p.CreadoEn, p.AnuladoPorUserId, p.AnuladoEn, p.MotivoAnulacion,
                       u.Nombre as CreadoPorNombre, u.Apellido as CreadoPorApellido
                FROM Pagos p
                LEFT JOIN Usuarios u ON p.CreadoPorUserId = u.Id
                WHERE p.ContratoId = @contratoId AND p.Eliminado = 0
                ORDER BY p.FechaPago DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@contratoId", contratoId }
            };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                pagos.Add(MapPagoFromReader(reader));
            }

            return pagos;
        }

        public int CrearPago(Pago pago)
        {
            var query = @"
                INSERT INTO Pagos (ContratoId, Monto, FechaPago, Periodo, Observaciones, CreadoPorUserId)
                VALUES (@contratoId, @monto, @fechaPago, @periodo, @observaciones, @creadoPorUserId)";

            var parameters = new Dictionary<string, object>
            {
                { "@contratoId", pago.ContratoId },
                { "@monto", pago.Monto },
                { "@fechaPago", pago.FechaPago },
                { "@periodo", pago.Periodo },
                { "@observaciones", pago.Observaciones ?? (object)DBNull.Value },
                { "@creadoPorUserId", pago.CreadoPorUserId }
            };

            return ExecuteNonQuery(query, parameters);
        }

        private Pago MapPagoFromReader(IDataReader reader)
        {
            return new Pago
            {
                Id = reader.GetInt32("Id"),
                ContratoId = reader.GetInt32("ContratoId"),
                Monto = reader.GetDecimal("Monto"),
                FechaPago = reader.GetDateTime("FechaPago"),
                Periodo = reader.GetString("Periodo"),
                Observaciones = reader.IsDBNull("Observaciones") ? null : reader.GetString("Observaciones"),
                CreadoPorUserId = reader.GetInt32("CreadoPorUserId"),
                CreadoEn = reader.GetDateTime("CreadoEn"),
                AnuladoPorUserId = reader.IsDBNull("AnuladoPorUserId") ? null : reader.GetInt32("AnuladoPorUserId"),
                AnuladoEn = reader.IsDBNull("AnuladoEn") ? null : reader.GetDateTime("AnuladoEn"),
                MotivoAnulacion = reader.IsDBNull("MotivoAnulacion") ? null : reader.GetString("MotivoAnulacion"),
                CreadoPorUser = new Usuario
                {
                    Id = reader.GetInt32("CreadoPorUserId"),
                    Nombre = reader.GetString("CreadoPorNombre"),
                    Apellido = reader.GetString("CreadoPorApellido")
                }
            };
        }
    }
}