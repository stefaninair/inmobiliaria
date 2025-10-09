using Inmobiliaria.Data;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioTipoInmueble : BaseRepository
    {
        public RepositorioTipoInmueble(DatabaseConnection dbConnection) : base(dbConnection)
        {
        }

        public List<TipoInmueble> ObtenerTodos()
        {
            var tiposInmueble = new List<TipoInmueble>();
            var query = "SELECT Id, Nombre FROM TiposInmueble ORDER BY Id";

            using var reader = ExecuteReader(query);
            while (reader.Read())
            {
                tiposInmueble.Add(MapFromReader(reader));
            }

            return tiposInmueble;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            var query = "SELECT Id, Nombre FROM TiposInmueble WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            using var reader = ExecuteReader(query, parameters);
            if (reader.Read())
            {
                return MapFromReader(reader);
            }

            return null;
        }

        public int Alta(TipoInmueble t)
        {
            var query = "INSERT INTO TiposInmueble (Nombre) VALUES (@nombre)";
            var parameters = new Dictionary<string, object>
            {
                { "@nombre", t.Nombre }
            };

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = CreateCommand(query, connection, parameters);
            command.ExecuteNonQuery();
            
            return GetLastInsertId(connection);
        }

        public int Modificacion(TipoInmueble t)
        {
            var query = "UPDATE TiposInmueble SET Nombre = @nombre WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", t.Id },
                { "@nombre", t.Nombre }
            };

            return ExecuteNonQuery(query, parameters);
        }

        public int Baja(int id)
        {
            var query = "DELETE FROM TiposInmueble WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            return ExecuteNonQuery(query, parameters);
        }

        private TipoInmueble MapFromReader(IDataReader reader)
        {
            return new TipoInmueble
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre")
            };
        }
    }
}