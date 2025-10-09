using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Inmobiliaria.Services
{
    public class PagoService : BaseRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagoService(DatabaseConnection dbConnection, IHttpContextAccessor httpContextAccessor) : base(dbConnection)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> CrearPagoAsync(Pago pago)
        {
            var query = @"
                INSERT INTO Pagos (ContratoId, Monto, FechaPago, Periodo, Observaciones, CreadoPorUserId) 
                VALUES (@contratoId, @monto, @fechaPago, @periodo, @observaciones, @creadoPorUserId)";
            
            var parameters = new Dictionary<string, object>
            {
                { "@contratoId", pago.ContratoId },
                { "@monto", pago.Monto },
                { "@fechaPago", pago.FechaPago },
                { "@periodo", pago.Periodo },
                { "@observaciones", pago.Observaciones ?? (object)DBNull.Value },
                { "@creadoPorUserId", GetCurrentUserId() }
            };

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = CreateCommand(query, connection, parameters);
            command.ExecuteNonQuery();
            
            return GetLastInsertId(connection);
        }

        public async Task<bool> AnularPagoAsync(int pagoId, string motivo)
        {
            var query = @"
                UPDATE Pagos 
                SET AnuladoPorUserId = @anuladoPorUserId, AnuladoEn = @anuladoEn, MotivoAnulacion = @motivo 
                WHERE Id = @pagoId AND AnuladoEn IS NULL";
            
            var parameters = new Dictionary<string, object>
            {
                { "@pagoId", pagoId },
                { "@anuladoPorUserId", GetCurrentUserId() },
                { "@anuladoEn", DateTime.Now },
                { "@motivo", motivo }
            };

            var rowsAffected = ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> EliminarPagoAsync(int pagoId)
        {
            var query = @"
                UPDATE Pagos 
                SET Eliminado = 1, EliminadoPorUserId = @eliminadoPorUserId, EliminadoEn = @eliminadoEn 
                WHERE Id = @pagoId AND Eliminado = 0";
            
            var parameters = new Dictionary<string, object>
            {
                { "@pagoId", pagoId },
                { "@eliminadoPorUserId", GetCurrentUserId() },
                { "@eliminadoEn", DateTime.Now }
            };

            var rowsAffected = ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> RestaurarPagoAsync(int pagoId)
        {
            var query = @"
                UPDATE Pagos 
                SET Eliminado = 0, EliminadoPorUserId = NULL, EliminadoEn = NULL 
                WHERE Id = @pagoId AND Eliminado = 1";
            
            var parameters = new Dictionary<string, object>
            {
                { "@pagoId", pagoId }
            };

            var rowsAffected = ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<List<Pago>> ObtenerPagosActivosAsync()
        {
            var pagos = new List<Pago>();
            var query = @"
                SELECT p.Id, p.ContratoId, p.Monto, p.FechaPago, p.Periodo, p.Observaciones, 
                       p.CreadoPorUserId, p.CreadoEn, p.AnuladoPorUserId, p.AnuladoEn, p.MotivoAnulacion,
                       p.Eliminado, p.EliminadoPorUserId, p.EliminadoEn,
                       c.MontoMensual, c.FechaInicio, c.FechaFin
                FROM Pagos p
                LEFT JOIN Contratos c ON p.ContratoId = c.Id
                WHERE p.Eliminado = 0 AND p.AnuladoEn IS NULL
                ORDER BY p.FechaPago DESC";

            using var reader = ExecuteReader(query);
            while (reader.Read())
            {
                pagos.Add(MapFromReader(reader));
            }

            return pagos;
        }

        public async Task<List<Pago>> ObtenerPagosEliminadosAsync()
        {
            var pagos = new List<Pago>();
            var query = @"
                SELECT p.Id, p.ContratoId, p.Monto, p.FechaPago, p.Periodo, p.Observaciones, 
                       p.CreadoPorUserId, p.CreadoEn, p.AnuladoPorUserId, p.AnuladoEn, p.MotivoAnulacion,
                       p.Eliminado, p.EliminadoPorUserId, p.EliminadoEn,
                       c.MontoMensual, c.FechaInicio, c.FechaFin
                FROM Pagos p
                LEFT JOIN Contratos c ON p.ContratoId = c.Id
                WHERE p.Eliminado = 1
                ORDER BY p.EliminadoEn DESC";

            using var reader = ExecuteReader(query);
            while (reader.Read())
            {
                pagos.Add(MapFromReader(reader));
            }

            return pagos;
        }

        public async Task<List<Pago>> ObtenerPagosPorContratoAsync(int contratoId)
        {
            var pagos = new List<Pago>();
            var query = @"
                SELECT p.Id, p.ContratoId, p.Monto, p.FechaPago, p.Periodo, p.Observaciones, 
                       p.CreadoPorUserId, p.CreadoEn, p.AnuladoPorUserId, p.AnuladoEn, p.MotivoAnulacion,
                       p.Eliminado, p.EliminadoPorUserId, p.EliminadoEn,
                       c.MontoMensual, c.FechaInicio, c.FechaFin
                FROM Pagos p
                LEFT JOIN Contratos c ON p.ContratoId = c.Id
                WHERE p.ContratoId = @contratoId AND p.Eliminado = 0
                ORDER BY p.FechaPago DESC";
            var parameters = new Dictionary<string, object> { { "@contratoId", contratoId } };

            using var reader = ExecuteReader(query, parameters);
            while (reader.Read())
            {
                pagos.Add(MapFromReader(reader));
            }

            return pagos;
        }

        public async Task<decimal> CalcularTotalPagosAsync(int contratoId)
        {
            var query = @"
                SELECT COALESCE(SUM(Monto), 0) 
                FROM Pagos 
                WHERE ContratoId = @contratoId AND Eliminado = 0 AND AnuladoEn IS NULL";
            var parameters = new Dictionary<string, object> { { "@contratoId", contratoId } };

            var result = ExecuteScalar(query, parameters);
            return Convert.ToDecimal(result);
        }

        public async Task<bool> ExistePagoParaPeriodoAsync(int contratoId, string periodo)
        {
            var query = @"
                SELECT COUNT(*) 
                FROM Pagos 
                WHERE ContratoId = @contratoId AND Periodo = @periodo AND Eliminado = 0 AND AnuladoEn IS NULL";
            var parameters = new Dictionary<string, object>
            {
                { "@contratoId", contratoId },
                { "@periodo", periodo }
            };

            var count = ExecuteScalar(query, parameters);
            return Convert.ToInt32(count) > 0;
        }

        private Pago MapFromReader(IDataReader reader)
        {
            return new Pago
            {
                Id = reader.GetInt32("Id"),
                ContratoId = reader.GetInt32("ContratoId"),
                Monto = reader.GetDecimal("Monto"),
                FechaPago = reader.GetDateTime("FechaPago"),
                Periodo = reader.GetString("Periodo"),
                Observaciones = reader.IsDBNull("Observaciones") ? null : reader.GetString("Observaciones"),
                CreadoPorUserId = reader.GetInt32("CreadoPorUserId"),
                CreadoEn = reader.GetDateTime("CreadoEn"),
                AnuladoPorUserId = reader.IsDBNull("AnuladoPorUserId") ? null : reader.GetInt32("AnuladoPorUserId"),
                AnuladoEn = reader.IsDBNull("AnuladoEn") ? null : reader.GetDateTime("AnuladoEn"),
                MotivoAnulacion = reader.IsDBNull("MotivoAnulacion") ? null : reader.GetString("MotivoAnulacion"),
                Eliminado = reader.IsDBNull("Eliminado") ? false : reader.GetBoolean("Eliminado"),
                EliminadoPorUserId = reader.IsDBNull("EliminadoPorUserId") ? null : reader.GetInt32("EliminadoPorUserId"),
                EliminadoEn = reader.IsDBNull("EliminadoEn") ? null : reader.GetDateTime("EliminadoEn"),
                Contrato = reader.IsDBNull("ContratoId") ? null : new Contrato
                {
                    Id = reader.GetInt32("ContratoId"),
                    MontoMensual = reader.IsDBNull("MontoMensual") ? 0 : reader.GetDecimal("MontoMensual"),
                    FechaInicio = reader.IsDBNull("FechaInicio") ? DateTime.MinValue : reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.IsDBNull("FechaFin") ? DateTime.MinValue : reader.GetDateTime("FechaFin")
                }
            };
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 1;
        }
    }
}