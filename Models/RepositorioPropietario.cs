using Inmobiliaria.Data;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioPropietario : BaseRepository
    {
        public RepositorioPropietario(DatabaseConnection dbConnection) : base(dbConnection)
        {
        }

        public List<Propietario> ObtenerTodos()
        {
            var propietarios = new List<Propietario>();
            var query = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave FROM Propietarios ORDER BY Id";

            using var reader = ExecuteReader(query);
            while (reader.Read())
            {
                propietarios.Add(MapFromReader(reader));
            }

            return propietarios;
        }

        public Propietario? ObtenerPorId(int id)
        {
            var query = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave FROM Propietarios WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            using var reader = ExecuteReader(query, parameters);
            if (reader.Read())
            {
                return MapFromReader(reader);
            }

            return null;
        }

        public int Alta(Propietario p)
        {
            var query = "INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email, Clave) VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave)";
            var parameters = new Dictionary<string, object>
            {
                { "@nombre", p.Nombre },
                { "@apellido", p.Apellido },
                { "@dni", p.Dni },
                { "@telefono", p.Telefono ?? (object)DBNull.Value },
                { "@email", p.Email ?? (object)DBNull.Value },
                { "@clave", p.Clave }
            };

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = CreateCommand(query, connection, parameters);
            command.ExecuteNonQuery();
            
            return GetLastInsertId(connection);
        }

        public int Modificacion(Propietario p)
        {
            var query = "UPDATE Propietarios SET Nombre = @nombre, Apellido = @apellido, Dni = @dni, Telefono = @telefono, Email = @email, Clave = @clave WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", p.Id },
                { "@nombre", p.Nombre },
                { "@apellido", p.Apellido },
                { "@dni", p.Dni },
                { "@telefono", p.Telefono ?? (object)DBNull.Value },
                { "@email", p.Email ?? (object)DBNull.Value },
                { "@clave", p.Clave }
            };

            return ExecuteNonQuery(query, parameters);
        }

        public int Baja(int id)
        {
            var query = "DELETE FROM Propietarios WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            return ExecuteNonQuery(query, parameters);
        }

        public List<Propietario> ObtenerPaginados(int pagina, int tamanoPagina)
        {
            var propietarios = new List<Propietario>();
            var offset = (pagina - 1) * tamanoPagina;
            var query = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave FROM Propietarios ORDER BY Id LIMIT @offset, @tamano";
            var parameters = new Dictionary<string, object>
            {
                { "@offset", offset },
                { "@tamano", tamanoPagina }
            };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                propietarios.Add(MapFromReader(reader));
            }

            return propietarios;
        }

        public int Contar()
        {
            var query = "SELECT COUNT(*) FROM Propietarios";
            var result = ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        private Propietario MapFromReader(IDataReader reader)
        {
            return new Propietario
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                Apellido = reader.GetString("Apellido"),
                Dni = reader.GetString("Dni"),
                Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                Clave = reader.GetString("Clave")
            };
        }
    }
}