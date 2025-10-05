using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;
using Inmobiliaria.Data;
using Inmobiliaria.Services;
using System.Security.Claims;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PagoService _pagoService;

        public PagosController(ApplicationDbContext context, PagoService pagoService)
        {
            _context = context;
            _pagoService = pagoService;
        }

        // GET: Pagos
        public async Task<IActionResult> Index()
        {
            var pagos = await _pagoService.ObtenerPagosActivosAsync();
            return View(pagos);
        }

        // GET: Pagos/Eliminados
        [Authorize(Policy = "SoloAdmin")]
        public async Task<IActionResult> Eliminados()
        {
            var pagos = await _pagoService.ObtenerPagosEliminadosAsync();
            ViewBag.Titulo = "Pagos Eliminados";
            return View("Index", pagos);
        }

        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inmueble)
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inquilino)
                .Include(p => p.CreadoPorUser)
                .Include(p => p.AnuladoPorUser)
                .Include(p => p.EliminadoPorUser)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null)
                return NotFound();

            return View(pago);
        }

        // GET: Pagos/Create
        public async Task<IActionResult> Create(int? contratoId)
        {
            await CargarListas();
            
            var pago = new Pago
            {
                FechaPago = DateTime.Today,
                Periodo = DateTime.Now.ToString("yyyy-MM")
            };

            if (contratoId.HasValue)
            {
                pago.ContratoId = contratoId.Value;
            }

            return View(pago);
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validar que no exista pago para el mismo período
                    if (await _pagoService.ExistePagoParaPeriodoAsync(pago.ContratoId, pago.Periodo))
                    {
                        ModelState.AddModelError("", "Ya existe un pago para este contrato en el período seleccionado.");
                        await CargarListas();
                        return View(pago);
                    }

                    await _pagoService.CrearPagoAsync(pago);
                    TempData["Success"] = "Pago registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                await CargarListas();
                return View(pago);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                await CargarListas();
                return View(pago);
            }
        }

        // GET: Pagos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inmueble)
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inquilino)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null || pago.Eliminado || pago.Anulado)
                return NotFound();

            await CargarListas();
            return View(pago);
        }

        // POST: Pagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pago pago)
        {
            try
            {
                if (id != pago.Id)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    // Validar que no exista otro pago para el mismo período
                    var pagoExistente = await _context.Pagos
                        .FirstOrDefaultAsync(p => p.ContratoId == pago.ContratoId && 
                                               p.Periodo == pago.Periodo && 
                                               p.Id != pago.Id && 
                                               !p.Eliminado && 
                                               !p.Anulado);

                    if (pagoExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un pago para este contrato en el período seleccionado.");
                        await CargarListas();
                        return View(pago);
                    }

                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Pago modificado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                await CargarListas();
                return View(pago);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                await CargarListas();
                return View(pago);
            }
        }

        // GET: Pagos/Anular/5
        public async Task<IActionResult> Anular(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inmueble)
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inquilino)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null || pago.Eliminado || pago.Anulado)
                return NotFound();

            return View(pago);
        }

        // POST: Pagos/Anular/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anular(int id, string motivoAnulacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(motivoAnulacion))
                {
                    ModelState.AddModelError("", "El motivo de anulación es requerido.");
                    var pago = await _context.Pagos
                        .Include(p => p.Contrato!)
                            .ThenInclude(c => c.Inmueble)
                        .Include(p => p.Contrato!)
                            .ThenInclude(c => c.Inquilino)
                        .FirstOrDefaultAsync(p => p.Id == id);
                    return View(pago);
                }

                var resultado = await _pagoService.AnularPagoAsync(id, motivoAnulacion);
                if (resultado)
                {
                    TempData["Success"] = "Pago anulado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo anular el pago.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Pagos/Delete/5
        [Authorize(Policy = "SoloAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inmueble)
                .Include(p => p.Contrato!)
                    .ThenInclude(c => c.Inquilino)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null || pago.Eliminado)
                return NotFound();

            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SoloAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var resultado = await _pagoService.EliminarPagoAsync(id);
                if (resultado)
                {
                    TempData["Success"] = "Pago eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el pago.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Pagos/Restaurar/5
        [Authorize(Policy = "SoloAdmin")]
        public async Task<IActionResult> Restaurar(int id)
        {
            try
            {
                var resultado = await _pagoService.RestaurarPagoAsync(id);
                if (resultado)
                {
                    TempData["Success"] = "Pago restaurado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo restaurar el pago.";
                }

                return RedirectToAction(nameof(Eliminados));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction(nameof(Eliminados));
            }
        }

        // GET: Pagos/PorContrato/5
        public async Task<IActionResult> PorContrato(int contratoId)
        {
            var pagos = await _pagoService.ObtenerPagosPorContratoAsync(contratoId);
            var contrato = await _context.Contratos
                .Include(c => c.Inmueble)
                .Include(c => c.Inquilino)
                .FirstOrDefaultAsync(c => c.Id == contratoId);

            ViewBag.Contrato = contrato;
            ViewBag.TotalPagos = await _pagoService.CalcularTotalPagosAsync(contratoId);
            return View(pagos);
        }

        private async Task CargarListas()
        {
            ViewBag.Contratos = await _context.Contratos
                .Include(c => c.Inmueble)
                .Include(c => c.Inquilino)
                .Where(c => !c.FechaTerminacionAnticipada.HasValue && 
                           DateTime.Now >= c.FechaInicio && 
                           DateTime.Now <= c.FechaFin)
                .OrderBy(c => c.Inmueble!.Direccion)
                .ToListAsync();
        }
    }
}

