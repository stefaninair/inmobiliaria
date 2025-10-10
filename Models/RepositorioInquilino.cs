using Inmobiliaria.Data;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioInquilino : BaseRepository
    {
        public RepositorioInquilino(DatabaseConnection dbConnection) : base(dbConnection)
        {
        }

        public List<Inquilino> ObtenerTodos()
        {
            var inquilinos = new List<Inquilino>();
            var query = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM Inquilinos ORDER BY Id";

            using var reader = ExecuteReader(query);
            while (reader.Read())
            {
                inquilinos.Add(MapFromReader(reader));
            }

            return inquilinos;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            var query = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM Inquilinos WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            using var reader = ExecuteReader(query, parameters);
            if (reader.Read())
            {
                return MapFromReader(reader);
            }

            return null;
        }

        public int Alta(Inquilino i)
        {
            var query = "INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email) VALUES (@nombre, @apellido, @dni, @telefono, @email)";
            var parameters = new Dictionary<string, object>
            {
                { "@nombre", i.Nombre },
                { "@apellido", i.Apellido },
                { "@dni", i.Dni },
                { "@telefono", i.Telefono ?? (object)DBNull.Value },
                { "@email", i.Email ?? (object)DBNull.Value }
            };

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = CreateCommand(query, connection, parameters);
            command.ExecuteNonQuery();
            
            return GetLastInsertId(connection);
        }

        public int Modificacion(Inquilino i)
        {
            var query = "UPDATE Inquilinos SET Nombre = @nombre, Apellido = @apellido, Dni = @dni, Telefono = @telefono, Email = @email WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", i.Id },
                { "@nombre", i.Nombre },
                { "@apellido", i.Apellido },
                { "@dni", i.Dni },
                { "@telefono", i.Telefono ?? (object)DBNull.Value },
                { "@email", i.Email ?? (object)DBNull.Value }
            };

            return ExecuteNonQuery(query, parameters);
        }

        public int Baja(int id)
        {
            var query = "DELETE FROM Inquilinos WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            return ExecuteNonQuery(query, parameters);
        }

        public List<Inquilino> ObtenerPaginados(int pagina, int tamanoPagina)
        {
            var inquilinos = new List<Inquilino>();
            var offset = (pagina - 1) * tamanoPagina;
            var query = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM Inquilinos ORDER BY Id LIMIT @offset, @tamano";
            var parameters = new Dictionary<string, object>
            {
                { "@offset", offset },
                { "@tamano", tamanoPagina }
            };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                inquilinos.Add(MapFromReader(reader));
            }

            return inquilinos;
        }

        public int Contar()
        {
            var query = "SELECT COUNT(*) FROM Inquilinos";
            var result = ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public List<Inquilino> BuscarPorNombre(string nombre)
        {
            var inquilinos = new List<Inquilino>();
            var query = @"
                SELECT Id, Nombre, Apellido, Dni, Telefono, Email 
                FROM Inquilinos
                WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre
                ORDER BY Nombre, Apellido";
            var parameters = new Dictionary<string, object> { { "@nombre", $"%{nombre}%" } };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                inquilinos.Add(MapFromReader(reader));
            }

            return inquilinos;
        }

        private Inquilino MapFromReader(IDataReader reader)
        {
            return new Inquilino
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                Apellido = reader.GetString("Apellido"),
                Dni = reader.GetString("Dni"),
                Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email")
            };
        }
    }
}