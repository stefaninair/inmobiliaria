using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System;

namespace Inmobiliaria.Models
{


    public class RepositorioInmueble
    {
        private readonly string connectionString;


        public RepositorioInmueble(IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encuentra configurada en appsettings.json.");
            }
            this.connectionString = connStr;
        }


        public List<Inmueble> ObtenerTodos()
        {
            var res = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT I.Id, I.Direccion, I.Ambientes, I.Superficie, I.Latitud, I.Longitud, I.PropietarioId, I.Habilitado, " +
                          "P.Id, P.Nombre, P.Apellido FROM Inmuebles I INNER JOIN Propietarios P ON I.PropietarioId = P.Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Se lee el Id del Inmueble correctamente.
                            int inmuebleId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32("Id");

                            // Se lee el Id del Propietario correctamente.
                            int propietarioId = reader.IsDBNull(reader.GetOrdinal("PropietarioId")) ? 0 : reader.GetInt32("PropietarioId");

                            res.Add(new Inmueble
                            {
                                Id = inmuebleId,
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? "" : reader.GetString("Direccion"),
                                Ambientes = reader.IsDBNull(reader.GetOrdinal("Ambientes")) ? 0 : reader.GetInt32("Ambientes"),
                                Superficie = reader.IsDBNull(reader.GetOrdinal("Superficie")) ? 0 : reader.GetInt32("Superficie"),
                                Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? 0 : reader.GetDecimal("Latitud"),
                                Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? 0 : reader.GetDecimal("Longitud"),
                                PropietarioId = propietarioId,
                                // Se crea el objeto Duenio y se asigna su Id directamente del valor leído.
                                Duenio = new Propietario
                                {
                                    Id = propietarioId,
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? "" : reader.GetString("Nombre"),
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("Apellido")) ? "" : reader.GetString("Apellido"),
                                },
                                Habilitado = reader.IsDBNull(reader.GetOrdinal("Habilitado")) ? false : reader.GetBoolean("Habilitado")
                            });
                        }
                    }
                }
            }
            return res;
        }
        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? i = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT I.Id, I.Direccion, I.Ambientes, I.Superficie, I.Latitud, I.Longitud, I.PropietarioId, I.Habilitado, " +
                     "P.Id AS PropietarioId, P.Nombre, P.Apellido FROM Inmuebles I INNER JOIN Propietarios P ON I.PropietarioId = P.Id WHERE I.Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            i = new Inmueble
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32("Id"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? "" : reader.GetString("Direccion"),
                                Ambientes = reader.IsDBNull(reader.GetOrdinal("Ambientes")) ? 0 : reader.GetInt32("Ambientes"),
                                Superficie = reader.IsDBNull(reader.GetOrdinal("Superficie")) ? 0 : reader.GetInt32("Superficie"),
                                Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? 0 : reader.GetDecimal("Latitud"),
                                Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? 0 : reader.GetDecimal("Longitud"),
                                PropietarioId = reader.IsDBNull(reader.GetOrdinal("PropietarioId")) ? 0 : reader.GetInt32("PropietarioId"),
                                Duenio = new Propietario
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("PropietarioId")) ? 0 : reader.GetInt32("PropietarioId"),
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? "" : reader.GetString("Nombre"),
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("Apellido")) ? "" : reader.GetString("Apellido"),
                                },
                                Habilitado = reader.IsDBNull(reader.GetOrdinal("Habilitado")) ? false : reader.GetBoolean("Habilitado")
                            };
                        }
                    }
                }
            }
            return i;
        }

        public int Alta(Inmueble i)
        {
            var id = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "INSERT INTO Inmuebles (Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId, Habilitado) " +
                     "VALUES (@direccion, @ambientes, @superficie, @latitud, @longitud, @propietarioId, @habilitado);" +
                     "SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@ambientes", i.Ambientes);
                    command.Parameters.AddWithValue("@superficie", i.Superficie);
                    command.Parameters.AddWithValue("@latitud", i.Latitud);
                    command.Parameters.AddWithValue("@longitud", i.Longitud);
                    command.Parameters.AddWithValue("@propietarioId", i.PropietarioId);
                    command.Parameters.AddWithValue("@habilitado", i.Habilitado);
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    i.Id = id;
                }
            }
            return id;
        }

        public int Modificacion(Inmueble i)
        {
            var filas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE Inmuebles SET Direccion=@direccion, Ambientes=@ambientes, Superficie=@superficie, Latitud=@latitud, Longitud=@longitud, PropietarioId=@propietarioId, Habilitado=@habilitado WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@ambientes", i.Ambientes);
                    command.Parameters.AddWithValue("@superficie", i.Superficie);
                    command.Parameters.AddWithValue("@latitud", i.Latitud);
                    command.Parameters.AddWithValue("@longitud", i.Longitud);
                    command.Parameters.AddWithValue("@propietarioId", i.PropietarioId);
                    command.Parameters.AddWithValue("@habilitado", i.Habilitado);
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
                var sql = "DELETE FROM Inmuebles WHERE Id = @id";
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
