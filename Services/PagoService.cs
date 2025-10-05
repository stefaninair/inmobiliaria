using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using System.Security.Claims;

namespace Inmobiliaria.Services
{
    public class PagoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagoService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Pago> CrearPagoAsync(Pago pago)
        {
            // Asignar datos de auditoría
            pago.CreadoPorUserId = GetCurrentUserId();
            pago.CreadoEn = DateTime.Now;
            pago.Eliminado = false;

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return pago;
        }

        public async Task<bool> AnularPagoAsync(int pagoId, string motivoAnulacion)
        {
            var pago = await _context.Pagos.FindAsync(pagoId);
            if (pago == null || pago.Anulado || pago.Eliminado)
                return false;

            pago.AnuladoPorUserId = GetCurrentUserId();
            pago.AnuladoEn = DateTime.Now;
            pago.MotivoAnulacion = motivoAnulacion;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarPagoAsync(int pagoId)
        {
            var pago = await _context.Pagos.FindAsync(pagoId);
            if (pago == null || pago.Eliminado)
                return false;

            pago.Eliminado = true;
            pago.EliminadoPorUserId = GetCurrentUserId();
            pago.EliminadoEn = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestaurarPagoAsync(int pagoId)
        {
            var pago = await _context.Pagos.FindAsync(pagoId);
            if (pago == null || !pago.Eliminado)
                return false;

            pago.Eliminado = false;
            pago.EliminadoPorUserId = null;
            pago.EliminadoEn = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Pago>> ObtenerPagosActivosAsync()
        {
            return await _context.Pagos
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inmueble)
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inquilino)
                .Include(p => p.CreadoPorUser)
                .Include(p => p.AnuladoPorUser)
                .Where(p => !p.Eliminado)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        public async Task<List<Pago>> ObtenerPagosEliminadosAsync()
        {
            return await _context.Pagos
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inmueble)
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inquilino)
                .Include(p => p.CreadoPorUser)
                .Include(p => p.EliminadoPorUser)
                .Where(p => p.Eliminado)
                .OrderByDescending(p => p.EliminadoEn)
                .ToListAsync();
        }

        public async Task<List<Pago>> ObtenerPagosPorContratoAsync(int contratoId)
        {
            return await _context.Pagos
                .Include(p => p.Contrato)
                .Include(p => p.CreadoPorUser)
                .Include(p => p.AnuladoPorUser)
                .Where(p => p.ContratoId == contratoId && !p.Eliminado)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        public async Task<decimal> CalcularTotalPagosAsync(int contratoId)
        {
            return await _context.Pagos
                .Where(p => p.ContratoId == contratoId && !p.Eliminado && !p.Anulado)
                .SumAsync(p => p.Monto);
        }

        public async Task<bool> ExistePagoParaPeriodoAsync(int contratoId, string periodo)
        {
            return await _context.Pagos
                .AnyAsync(p => p.ContratoId == contratoId && 
                              p.Periodo == periodo && 
                              !p.Eliminado && 
                              !p.Anulado);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 1;
        }
    }
}
