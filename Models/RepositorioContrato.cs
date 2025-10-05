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
                var sql = @"SELECT c.Id, c.InmuebleId, c.InquilinoId, c.FechaInicio, c.FechaFin, c.MontoMensual, 
                            c.FechaTerminacionAnticipada, c.Multa, c.CreadoPorUserId, c.CreadoEn, c.TerminadoPorUserId, c.TerminadoEn,
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
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                FechaTerminacionAnticipada = reader.IsDBNull(reader.GetOrdinal("FechaTerminacionAnticipada")) ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                                Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) ? null : reader.GetDecimal("Multa"),
                                CreadoPorUserId = reader.GetInt32("CreadoPorUserId"),
                                CreadoEn = reader.GetDateTime("CreadoEn"),
                                TerminadoPorUserId = reader.IsDBNull(reader.GetOrdinal("TerminadoPorUserId")) ? null : reader.GetInt32("TerminadoPorUserId"),
                                TerminadoEn = reader.IsDBNull(reader.GetOrdinal("TerminadoEn")) ? null : reader.GetDateTime("TerminadoEn"),
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
                var sql = @"SELECT c.Id, c.InmuebleId, c.InquilinoId, c.FechaInicio, c.FechaFin, c.MontoMensual, 
                            c.FechaTerminacionAnticipada, c.Multa, c.CreadoPorUserId, c.CreadoEn, c.TerminadoPorUserId, c.TerminadoEn,
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
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                FechaTerminacionAnticipada = reader.IsDBNull(reader.GetOrdinal("FechaTerminacionAnticipada")) ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                                Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) ? null : reader.GetDecimal("Multa"),
                                CreadoPorUserId = reader.GetInt32("CreadoPorUserId"),
                                CreadoEn = reader.GetDateTime("CreadoEn"),
                                TerminadoPorUserId = reader.IsDBNull(reader.GetOrdinal("TerminadoPorUserId")) ? null : reader.GetInt32("TerminadoPorUserId"),
                                TerminadoEn = reader.IsDBNull(reader.GetOrdinal("TerminadoEn")) ? null : reader.GetDateTime("TerminadoEn"),
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
                var sql = @"INSERT INTO contratos (InmuebleId, InquilinoId, FechaInicio, FechaFin, MontoMensual, 
                            FechaTerminacionAnticipada, Multa, CreadoPorUserId, CreadoEn, TerminadoPorUserId, TerminadoEn)
                            VALUES (@inmuebleId, @inquilinoId, @fechaInicio, @fechaFin, @montoMensual, 
                            @fechaTerminacionAnticipada, @multa, @creadoPorUserId, @creadoEn, @terminadoPorUserId, @terminadoEn);
                            SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("@montoMensual", c.MontoMensual);
                    command.Parameters.AddWithValue("@fechaTerminacionAnticipada", c.FechaTerminacionAnticipada ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@multa", c.Multa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@creadoPorUserId", c.CreadoPorUserId);
                    command.Parameters.AddWithValue("@creadoEn", c.CreadoEn);
                    command.Parameters.AddWithValue("@terminadoPorUserId", c.TerminadoPorUserId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@terminadoEn", c.TerminadoEn ?? (object)DBNull.Value);
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
                            FechaInicio=@fechaInicio, FechaFin=@fechaFin, MontoMensual=@montoMensual, 
                            FechaTerminacionAnticipada=@fechaTerminacionAnticipada, Multa=@multa,
                            TerminadoPorUserId=@terminadoPorUserId, TerminadoEn=@terminadoEn
                            WHERE Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("@montoMensual", c.MontoMensual);
                    command.Parameters.AddWithValue("@fechaTerminacionAnticipada", c.FechaTerminacionAnticipada ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@multa", c.Multa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@terminadoPorUserId", c.TerminadoPorUserId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@terminadoEn", c.TerminadoEn ?? (object)DBNull.Value);
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
