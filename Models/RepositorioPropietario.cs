using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;

namespace Inmobiliaria.Models
{
    public class RepositorioPropietario
    {
        private readonly string connectionString;

        public RepositorioPropietario(IConfiguration configuration)
        {
            // El problema está aquí. El método GetConnectionString podría devolver null
            // si la clave "DefaultConnection" no existe.
            var connStr = configuration.GetConnectionString("DefaultConnection");
            
            // Verificamos si la cadena de conexión es nula y lanzamos una excepción si es el caso.
            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encuentra configurada en appsettings.json.");
            }
            this.connectionString = connStr;
        }

        // El resto del código es el mismo...
         public List<Propietario> ObtenerTodos()
        {
            var res = new List<Propietario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM propietarios";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Propietario
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32("Id"),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? "" : reader.GetString("Nombre"),
                                Apellido = reader.IsDBNull(reader.GetOrdinal("Apellido")) ? "" : reader.GetString("Apellido"),
                                Dni = reader.IsDBNull(reader.GetOrdinal("Dni")) ? "" : reader.GetString("Dni"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? "" : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email")
                            });
                        }
                    }
                }
            }
            return res;
        }        public Propietario? ObtenerPorId(int id)
        {
            Propietario? p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM propietarios WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email")
                            };
                        }
                    }
                }
            }
            return p;
        }

        public int Alta(Propietario p)
        {
            var id = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "INSERT INTO propietarios (Nombre, Apellido, Dni, Telefono, Email) " +
                          "VALUES (@nombre, @apellido, @dni, @telefono, @email);" +
                          "SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    p.Id = id;
                }
            }
            return id;
        }

        public int Modificacion(Propietario p)
        {
            var filas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE propietarios SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@id", p.Id);
                    connection.Open();
                    filas = command.ExecuteNonQuery();
                }
            }
            return filas;
        }

        public int Baja(int id)
        {
            var filas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "DELETE FROM propietarios WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    filas = command.ExecuteNonQuery();
                }
            }
            return filas;
        }
    }
}
