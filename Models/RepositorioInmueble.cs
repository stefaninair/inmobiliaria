using Inmobiliaria.Data;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioInmueble : BaseRepository
    {
        public RepositorioInmueble(DatabaseConnection dbConnection) : base(dbConnection)
        {
        }

        public List<Inmueble> ObtenerTodos()
        {
            var inmuebles = new List<Inmueble>();
            var query = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible, 
                       i.PropietarioId, i.TipoInmuebleId, i.Observaciones, i.Portada,
                       p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email,
                       t.Nombre as TipoNombre
                FROM Inmuebles i
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN TiposInmueble t ON i.TipoInmuebleId = t.Id
                ORDER BY i.Id";

            using var reader = ExecuteReader(query);
            while (reader.Read())
            {
                inmuebles.Add(MapFromReader(reader));
            }

            return inmuebles;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            var query = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible, 
                       i.PropietarioId, i.TipoInmuebleId, i.Observaciones, i.Portada,
                       p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email,
                       t.Nombre as TipoNombre
                FROM Inmuebles i
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN TiposInmueble t ON i.TipoInmuebleId = t.Id
                WHERE i.Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            using var reader = ExecuteReader(query, parameters);
            if (reader.Read())
            {
                return MapFromReader(reader);
            }

            return null;
        }

        public int Alta(Inmueble i)
        {
            var query = "INSERT INTO Inmuebles (Direccion, Uso, Ambientes, Superficie, Precio, Disponible, PropietarioId, TipoInmuebleId, Observaciones) VALUES (@direccion, @uso, @ambientes, @superficie, @precio, @disponible, @propietarioId, @tipoInmuebleId, @observaciones)";
            var parameters = new Dictionary<string, object>
            {
                { "@direccion", i.Direccion },
                { "@uso", i.Uso },
                { "@ambientes", i.Ambientes },
                { "@superficie", i.Superficie },
                { "@precio", i.Precio },
                { "@disponible", i.Disponible },
                { "@propietarioId", i.PropietarioId },
                { "@tipoInmuebleId", i.TipoInmuebleId },
                { "@observaciones", i.Observaciones ?? (object)DBNull.Value }
            };

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = CreateCommand(query, connection, parameters);
            command.ExecuteNonQuery();
            
            return GetLastInsertId(connection);
        }

        public int Modificacion(Inmueble i)
        {
            var query = "UPDATE Inmuebles SET Direccion = @direccion, Uso = @uso, Ambientes = @ambientes, Superficie = @superficie, Precio = @precio, Disponible = @disponible, PropietarioId = @propietarioId, TipoInmuebleId = @tipoInmuebleId, Observaciones = @observaciones WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", i.Id },
                { "@direccion", i.Direccion },
                { "@uso", i.Uso },
                { "@ambientes", i.Ambientes },
                { "@superficie", i.Superficie },
                { "@precio", i.Precio },
                { "@disponible", i.Disponible },
                { "@propietarioId", i.PropietarioId },
                { "@tipoInmuebleId", i.TipoInmuebleId },
                { "@observaciones", i.Observaciones ?? (object)DBNull.Value }
            };

            return ExecuteNonQuery(query, parameters);
        }

        public int Baja(int id)
        {
            var query = "DELETE FROM Inmuebles WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            return ExecuteNonQuery(query, parameters);
        }

        public List<Inmueble> ObtenerPorPropietario(int propietarioId)
        {
            var inmuebles = new List<Inmueble>();
            var query = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible, 
                       i.PropietarioId, i.TipoInmuebleId, i.Observaciones,
                       p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email,
                       t.Nombre as TipoNombre
                FROM Inmuebles i
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN TiposInmueble t ON i.TipoInmuebleId = t.Id
                WHERE i.PropietarioId = @propietarioId
                ORDER BY i.Id";
            var parameters = new Dictionary<string, object> { { "@propietarioId", propietarioId } };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                inmuebles.Add(MapFromReader(reader));
            }

            return inmuebles;
        }

        public List<Inmueble> ObtenerDisponibles()
        {
            var inmuebles = new List<Inmueble>();
            var query = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible, 
                       i.PropietarioId, i.TipoInmuebleId, i.Observaciones,
                       p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email,
                       t.Nombre as TipoNombre
                FROM Inmuebles i
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN TiposInmueble t ON i.TipoInmuebleId = t.Id
                WHERE i.Disponible = 1
                ORDER BY i.Id";

            using var reader = ExecuteReader(query);
            while (reader.Read())
            {
                inmuebles.Add(MapFromReader(reader));
            }

            return inmuebles;
        }

        public List<Inmueble> ObtenerPorTipo(int tipoInmuebleId)
        {
            var inmuebles = new List<Inmueble>();
            var query = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible, 
                       i.PropietarioId, i.TipoInmuebleId, i.Observaciones, i.Portada,
                       p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email,
                       t.Nombre as TipoNombre
                FROM Inmuebles i
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN TiposInmueble t ON i.TipoInmuebleId = t.Id
                WHERE i.TipoInmuebleId = @tipoInmuebleId
                ORDER BY i.Id";
            var parameters = new Dictionary<string, object> { { "@tipoInmuebleId", tipoInmuebleId } };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                inmuebles.Add(MapFromReader(reader));
            }

            return inmuebles;
        }

        public List<Inmueble> ObtenerPaginados(int pagina, int tamanoPagina)
        {
            var inmuebles = new List<Inmueble>();
            var offset = (pagina - 1) * tamanoPagina;
            var query = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible, 
                       i.PropietarioId, i.TipoInmuebleId, i.Observaciones, i.Portada,
                       p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email,
                       t.Nombre as TipoNombre
                FROM Inmuebles i
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN TiposInmueble t ON i.TipoInmuebleId = t.Id
                ORDER BY i.Id LIMIT @offset, @tamano";
            var parameters = new Dictionary<string, object>
            {
                { "@offset", offset },
                { "@tamano", tamanoPagina }
            };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                inmuebles.Add(MapFromReader(reader));
            }

            return inmuebles;
        }

        public int Contar()
        {
            var query = "SELECT COUNT(*) FROM Inmuebles";
            var result = ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        private Inmueble MapFromReader(IDataReader reader)
        {
            return new Inmueble
            {
                Id = reader.GetInt32("Id"),
                Direccion = reader.GetString("Direccion"),
                Uso = reader.GetString("Uso"),
                Ambientes = reader.GetInt32("Ambientes"),
                Superficie = reader.GetDecimal("Superficie"),
                Precio = reader.GetDecimal("Precio"),
                Disponible = reader.GetBoolean("Disponible"),
                PropietarioId = reader.GetInt32("PropietarioId"),
                TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                Observaciones = reader.IsDBNull("Observaciones") ? null : reader.GetString("Observaciones"),
                Portada = reader.IsDBNull("Portada") ? null : reader.GetString("Portada"),
                Propietario = reader.IsDBNull("Nombre") ? null : new Propietario
                {
                    Id = reader.GetInt32("PropietarioId"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Dni = reader.GetString("Dni"),
                    Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email")
                },
                TipoInmueble = reader.IsDBNull("TipoNombre") ? null : new TipoInmueble
                {
                    Id = reader.GetInt32("TipoInmuebleId"),
                    Nombre = reader.GetString("TipoNombre")
                }
            };
        }

        public int ModificarPortada(int id, string url)
        {
            var query = @"
                UPDATE Inmuebles SET
                Portada=@portada
                WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@portada", String.IsNullOrEmpty(url) ? (object)DBNull.Value : url },
                { "@id", id }
            };
            return ExecuteNonQuery(query, parameters);
        }

        public List<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            var inmuebles = new List<Inmueble>();
            var query = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Ambientes, i.Superficie, i.Precio, i.Disponible, 
                       i.PropietarioId, i.TipoInmuebleId, i.Observaciones, i.Portada,
                       p.Nombre, p.Apellido, p.Dni, p.Telefono, p.Email,
                       t.Nombre as TipoNombre
                FROM Inmuebles i
                LEFT JOIN Propietarios p ON i.PropietarioId = p.Id
                LEFT JOIN TiposInmueble t ON i.TipoInmuebleId = t.Id
                WHERE i.Disponible = 1 
                AND i.Id NOT IN (
                    SELECT DISTINCT c.InmuebleId 
                    FROM Contratos c 
                    WHERE c.FechaTerminacionAnticipada IS NULL 
                    AND (
                        (c.FechaInicio <= @fechaFin AND c.FechaFin >= @fechaInicio)
                    )
                )
                ORDER BY i.Id";

            var parameters = new Dictionary<string, object>
            {
                { "@fechaInicio", fechaInicio },
                { "@fechaFin", fechaFin }
            };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                inmuebles.Add(MapFromReader(reader));
            }

            return inmuebles;
        }
    }
}