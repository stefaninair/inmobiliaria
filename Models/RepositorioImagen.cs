using Inmobiliaria.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioImagen : BaseRepository
    {
        public RepositorioImagen(DatabaseConnection dbConnection) : base(dbConnection)
        {
        }

        public int Alta(Imagen p)
        {
            var query = @"INSERT INTO Imagenes 
                (InmuebleId, Url) 
                VALUES (@inmuebleId, @url)";
            var parameters = new Dictionary<string, object>
            {
                { "@inmuebleId", p.InmuebleId },
                { "@url", p.Url }
            };
            return ExecuteNonQuery(query, parameters);
        }

        public int Baja(int id)
        {
            var query = "DELETE FROM Imagenes WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };
            return ExecuteNonQuery(query, parameters);
        }

        public int Modificacion(Imagen p)
        {
            var query = @"
                UPDATE Imagenes SET 
                    Url=@url
                WHERE Id=@id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", p.Id },
                { "@url", p.Url }
            };
            return ExecuteNonQuery(query, parameters);
        }

        public Imagen? ObtenerPorId(int id)
        {
            var query = "SELECT Id, InmuebleId, Url FROM Imagenes WHERE Id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            using var reader = ExecuteReader(query, parameters);
            if (reader.Read())
            {
                return MapFromReader(reader);
            }
            return null;
        }

        public IList<Imagen> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var imagenes = new List<Imagen>();
            var query = "SELECT Id, InmuebleId, Url FROM Imagenes ORDER BY Id LIMIT @limit OFFSET @offset";
            var parameters = new Dictionary<string, object>
            {
                { "@limit", tamPagina },
                { "@offset", (paginaNro - 1) * tamPagina }
            };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                imagenes.Add(MapFromReader(reader));
            }
            return imagenes;
        }

        public int ObtenerCantidad()
        {
            var query = "SELECT COUNT(Id) FROM Imagenes";
            using var reader = ExecuteReader(query);
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }
            return 0;
        }

        public IList<Imagen> BuscarPorInmueble(int inmuebleId)
        {
            var imagenes = new List<Imagen>();
            var query = "SELECT Id, InmuebleId, Url FROM Imagenes WHERE InmuebleId = @inmuebleId";
            var parameters = new Dictionary<string, object> { { "@inmuebleId", inmuebleId } };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                imagenes.Add(MapFromReader(reader));
            }
            return imagenes;
        }

        private Imagen MapFromReader(IDataReader reader)
        {
            return new Imagen
            {
                Id = reader.GetInt32("Id"),
                InmuebleId = reader.GetInt32("InmuebleId"),
                Url = reader.GetString("Url")
            };
        }
    }
}
