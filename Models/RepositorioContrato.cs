using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;

namespace Inmobiliaria.Models
{
    public class RepositorioContrato
    {
        private readonly string connectionString;

        public RepositorioContrato(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string no encontrada");
        }

        public List<Contrato> ObtenerTodos()
        {
            var res = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.InmuebleId, c.InquilinoId, c.FechaInicio, c.FechaFin, c.Monto, c.Vigente,
                            i.Direccion, iq.Nombre, iq.Apellido
                            FROM contratos c
                            JOIN inmuebles i ON c.InmuebleId = i.Id
                            JOIN inquilinos iq ON c.InquilinoId = iq.Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                Monto = reader.GetDecimal("Monto"),
                                Vigente = reader.GetBoolean("Vigente"),
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("InmuebleId"),
                                    Direccion = reader.GetString("Direccion")
                                },
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido")
                                }
                            });
                        }
                    }
                }
            }
            return res;
        }

        public Contrato? ObtenerPorId(int id)
        {
            Contrato? contrato = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.InmuebleId, c.InquilinoId, c.FechaInicio, c.FechaFin, c.Monto, c.Vigente,
                            i.Direccion, iq.Nombre, iq.Apellido
                            FROM contratos c
                            JOIN inmuebles i ON c.InmuebleId = i.Id
                            JOIN inquilinos iq ON c.InquilinoId = iq.Id
                            WHERE c.Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                Monto = reader.GetDecimal("Monto"),
                                Vigente = reader.GetBoolean("Vigente"),
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("InmuebleId"),
                                    Direccion = reader.GetString("Direccion")
                                },
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido")
                                }
                            };
                        }
                    }
                }
            }
            return contrato;
        }

        public int Alta(Contrato c)
        {
            int id = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO contratos (InmuebleId, InquilinoId, FechaInicio, FechaFin, Monto, Vigente)
                            VALUES (@inmuebleId, @inquilinoId, @fechaInicio, @fechaFin, @monto, @vigente);
                            SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("@monto", c.Monto);
                    command.Parameters.AddWithValue("@vigente", c.Vigente);
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    c.Id = id;
                }
            }
            return id;
        }

        public int Modificacion(Contrato c)
        {
            int filas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE contratos SET InmuebleId=@inmuebleId, InquilinoId=@inquilinoId,
                            FechaInicio=@fechaInicio, FechaFin=@fechaFin, Monto=@monto, Vigente=@vigente
                            WHERE Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("@monto", c.Monto);
                    command.Parameters.AddWithValue("@vigente", c.Vigente);
                    command.Parameters.AddWithValue("@id", c.Id);
                    connection.Open();
                    filas = command.ExecuteNonQuery();
                }
            }
            return filas;
        }

        public int Baja(int id)
        {
            int filas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "DELETE FROM contratos WHERE Id = @id";
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
