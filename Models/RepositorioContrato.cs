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
                WHERE c.InmuebleId = @inmuebleId AND c.FechaFin >= CURDATE()
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
                    Propietario = new Propietario
                    {
                        Nombre = reader.GetString("PropietarioNombre"),
                        Apellido = reader.GetString("PropietarioApellido"),
                        Dni = reader.GetString("PropietarioDni")
                    }
                },
                Inquilino = new Inquilino
                {
                    Id = reader.GetInt32("InquilinoId"),
                    Nombre = reader.GetString("InquilinoNombre"),
                    Apellido = reader.GetString("InquilinoApellido"),
                    Dni = reader.GetString("InquilinoDni")
                }
            };
        }
    }
}