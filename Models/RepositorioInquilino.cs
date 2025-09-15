using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System;

namespace Inmobiliaria.Models
{
    public class RepositorioInquilino
    {
        private readonly string connectionString;

        public RepositorioInquilino(IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encuentra configurada en appsettings.json.");
            }
            this.connectionString = connStr;
        }

        public List<Inquilino> ObtenerTodos()
        {
            var res = new List<Inquilino>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM inquilinos";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inquilino
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
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? i = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM inquilinos WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            i = new Inquilino
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32("Id"),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? "" : reader.GetString("Nombre"),
                                Apellido = reader.IsDBNull(reader.GetOrdinal("Apellido")) ? "" : reader.GetString("Apellido"),
                                Dni = reader.IsDBNull(reader.GetOrdinal("Dni")) ? "" : reader.GetString("Dni"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? "" : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email")
                            };
                        }
                    }
                }
            }
            return i;
        }

        public int Alta(Inquilino i)
        {
            var id = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "INSERT INTO inquilinos (Nombre, Apellido, Dni, Telefono, Email) " +
                          "VALUES (@nombre, @apellido, @dni, @telefono, @email);" +
                          "SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    i.Id = id;
                }
            }
            return id;
        }

        public int Modificacion(Inquilino i)
        {
            var filas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE inquilinos SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@id", i.Id);
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
                var sql = "DELETE FROM inquilinos WHERE Id = @id";
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
