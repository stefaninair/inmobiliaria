using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;
using Inmobiliaria.Data;
using System;
using System.Linq;
using System.Security.Claims;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class ContratosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContratosController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task CargarListas()
        {
            ViewBag.Inmuebles = await _context.Inmuebles
                .Include(i => i.Propietario)
                .Include(i => i.TipoInmueble)
                .Where(i => i.Disponible)
                .ToListAsync();
            ViewBag.Inquilinos = await _context.Inquilinos.ToListAsync();
        }

        // GET: Contratos
        public async Task<IActionResult> Index(int pagina = 1, int elementosPorPagina = 5)
        {
            var totalElementos = await _context.Contratos.CountAsync();
            var offset = (pagina - 1) * elementosPorPagina;
            
            var lista = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .Include(c => c.CreadoPorUser)
                .Include(c => c.TerminadoPorUser)
                .OrderByDescending(c => c.CreadoEn)
                .Skip(offset)
                .Take(elementosPorPagina)
                .ToListAsync();
                
            var paginacion = new PaginacionModel<Contrato>(lista, pagina, elementosPorPagina, totalElementos);
            return View(paginacion);
        }

        // GET: Contratos/Create
        public async Task<IActionResult> Create()
        {
            await CargarListas();
            return View();
        }

        // POST: Contratos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contrato contrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validar solapamiento de fechas
                    var solapamiento = await _context.Contratos
                        .Where(c => c.InmuebleId == contrato.InmuebleId && 
                                   c.FechaTerminacionAnticipada == null && // Solo contratos activos
                                   ((contrato.FechaInicio >= c.FechaInicio && contrato.FechaInicio <= c.FechaFin) ||
                                    (contrato.FechaFin >= c.FechaInicio && contrato.FechaFin <= c.FechaFin) ||
                                    (contrato.FechaInicio <= c.FechaInicio && contrato.FechaFin >= c.FechaFin)))
                        .FirstOrDefaultAsync();

                    if (solapamiento != null)
                    {
                        ModelState.AddModelError("", "Ya existe un contrato activo para este inmueble en el período seleccionado.");
                        await CargarListas();
                        return View(contrato);
                    }

                    // Validar que la fecha de inicio sea anterior a la fecha de fin
                    if (contrato.FechaInicio >= contrato.FechaFin)
                    {
                        ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
                        await CargarListas();
                        return View(contrato);
                    }

                    // Asignar datos de auditoría
                    contrato.CreadoPorUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
                    contrato.CreadoEn = DateTime.Now;

                    _context.Contratos.Add(contrato);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Contrato creado correctamente";
                    return RedirectToAction(nameof(Index));
                }

                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errores);
                await CargarListas();
                return View(contrato);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                await CargarListas();
                return View(contrato);
            }
        }

        // GET: Contratos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (contrato == null)
                return NotFound();

            await CargarListas();
            return View(contrato);
        }

        // POST: Contratos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contrato contrato)
        {
            try
            {
                if (id != contrato.Id)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    // Validar solapamiento de fechas (excluyendo el contrato actual)
                    var solapamiento = await _context.Contratos
                        .Where(c => c.InmuebleId == contrato.InmuebleId && 
                                   c.Id != contrato.Id &&
                                   c.FechaTerminacionAnticipada == null && // Solo contratos activos
                                   ((contrato.FechaInicio >= c.FechaInicio && contrato.FechaInicio <= c.FechaFin) ||
                                    (contrato.FechaFin >= c.FechaInicio && contrato.FechaFin <= c.FechaFin) ||
                                    (contrato.FechaInicio <= c.FechaInicio && contrato.FechaFin >= c.FechaFin)))
                        .FirstOrDefaultAsync();

                    if (solapamiento != null)
                    {
                        ModelState.AddModelError("", "Ya existe un contrato activo para este inmueble en el período seleccionado.");
                        await CargarListas();
                        return View(contrato);
                    }

                    // Validar que la fecha de inicio sea anterior a la fecha de fin
                    if (contrato.FechaInicio >= contrato.FechaFin)
                    {
                        ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
                        await CargarListas();
                        return View(contrato);
                    }

                    _context.Update(contrato);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Contrato modificado correctamente";
                    return RedirectToAction(nameof(Index));
                }

                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errores);
                await CargarListas();
                return View(contrato);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                await CargarListas();
                return View(contrato);
            }
        }

        // GET: Contratos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .Include(c => c.CreadoPorUser)
                .Include(c => c.TerminadoPorUser)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (contrato == null)
                return NotFound();

            return View(contrato);
        }

        // GET: Contratos/Delete/5
        [Authorize(Policy = "SoloAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (contrato == null)
                return NotFound();

            return View(contrato);
        }

        // POST: Contratos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SoloAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var contrato = await _context.Contratos.FindAsync(id);
                if (contrato != null)
                {
                    _context.Contratos.Remove(contrato);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Contrato eliminado correctamente";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                var contrato = await _context.Contratos
                    .Include(c => c.Inquilino)
                    .Include(c => c.Inmueble)
                    .FirstOrDefaultAsync(c => c.Id == id);
                return View(contrato);
            }
        }

        // GET: Contratos/Vigentes
        public async Task<IActionResult> Vigentes()
        {
            var contratosVigentes = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .Include(c => c.CreadoPorUser)
                .Where(c => c.FechaTerminacionAnticipada == null && 
                           DateTime.Now >= c.FechaInicio && 
                           DateTime.Now <= c.FechaFin)
                .OrderBy(c => c.FechaFin)
                .ToListAsync();

            ViewBag.Titulo = "Contratos Vigentes";
            ViewBag.Total = contratosVigentes.Count;
            return View("Reporte", contratosVigentes);
        }

        // GET: Contratos/Vencidos
        public async Task<IActionResult> Vencidos()
        {
            var contratosVencidos = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .Include(c => c.CreadoPorUser)
                .Where(c => c.FechaTerminacionAnticipada == null && 
                           DateTime.Now > c.FechaFin)
                .OrderByDescending(c => c.FechaFin)
                .ToListAsync();

            ViewBag.Titulo = "Contratos Vencidos";
            ViewBag.Total = contratosVencidos.Count;
            return View("Reporte", contratosVencidos);
        }

        // GET: Contratos/PorInmueble
        public async Task<IActionResult> PorInmueble(int? inmuebleId)
        {
            await CargarListas();
            
            if (inmuebleId.HasValue)
            {
                var contratosPorInmueble = await _context.Contratos
                    .Include(c => c.Inquilino)
                    .Include(c => c.Inmueble)
                    .Include(c => c.CreadoPorUser)
                    .Where(c => c.InmuebleId == inmuebleId.Value)
                    .OrderByDescending(c => c.FechaInicio)
                    .ToListAsync();

                ViewBag.Titulo = $"Contratos del Inmueble: {contratosPorInmueble.FirstOrDefault()?.Inmueble?.Direccion}";
                ViewBag.Total = contratosPorInmueble.Count;
                ViewBag.InmuebleSeleccionado = inmuebleId.Value;
                return View("Reporte", contratosPorInmueble);
            }

            ViewBag.Titulo = "Seleccionar Inmueble";
            return View("SeleccionarInmueble");
        }

        // GET: Contratos/Estadisticas
        public async Task<IActionResult> Estadisticas()
        {
            var estadisticas = new
            {
                TotalContratos = await _context.Contratos.CountAsync(),
                ContratosVigentes = await _context.Contratos
                    .Where(c => c.FechaTerminacionAnticipada == null && 
                               DateTime.Now >= c.FechaInicio && 
                               DateTime.Now <= c.FechaFin)
                    .CountAsync(),
                ContratosVencidos = await _context.Contratos
                    .Where(c => c.FechaTerminacionAnticipada == null && 
                               DateTime.Now > c.FechaFin)
                    .CountAsync(),
                ContratosTerminadosAnticipadamente = await _context.Contratos
                    .Where(c => c.FechaTerminacionAnticipada != null)
                    .CountAsync(),
                IngresosMensuales = await _context.Contratos
                    .Where(c => c.FechaTerminacionAnticipada == null && 
                               DateTime.Now >= c.FechaInicio && 
                               DateTime.Now <= c.FechaFin)
                    .SumAsync(c => c.MontoMensual)
            };

            return View(estadisticas);
        }

        // GET: Contratos/Renovar/5
        public async Task<IActionResult> Renovar(int id)
        {
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contrato == null || contrato.FechaTerminacionAnticipada != null)
                return NotFound();

            return View(contrato);
        }

        // POST: Contratos/Renovar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renovar(int id, DateTime nuevaFechaFin, decimal? nuevoMontoMensual)
        {
            try
            {
                var contrato = await _context.Contratos.FindAsync(id);
                if (contrato == null || contrato.FechaTerminacionAnticipada != null)
                    return NotFound();

                // Validar que la nueva fecha sea posterior a la fecha actual
                if (nuevaFechaFin <= DateTime.Now)
                {
                    ModelState.AddModelError("", "La nueva fecha de fin debe ser posterior a la fecha actual.");
                    var contratoCompleto = await _context.Contratos
                        .Include(c => c.Inquilino)
                        .Include(c => c.Inmueble)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    return View(contratoCompleto);
                }

                // Validar que la nueva fecha sea posterior a la fecha de fin original
                if (nuevaFechaFin <= contrato.FechaFin)
                {
                    ModelState.AddModelError("", "La nueva fecha de fin debe ser posterior a la fecha de fin original.");
                    var contratoCompleto = await _context.Contratos
                        .Include(c => c.Inquilino)
                        .Include(c => c.Inmueble)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    return View(contratoCompleto);
                }

                // Actualizar contrato
                contrato.FechaFin = nuevaFechaFin;
                if (nuevoMontoMensual.HasValue && nuevoMontoMensual.Value > 0)
                {
                    contrato.MontoMensual = nuevoMontoMensual.Value;
                }

                _context.Update(contrato);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Contrato renovado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var contrato = await _context.Contratos
                    .Include(c => c.Inquilino)
                    .Include(c => c.Inmueble)
                    .FirstOrDefaultAsync(c => c.Id == id);
                return View(contrato);
            }
        }

        // GET: Contratos/Terminar/5
        public async Task<IActionResult> Terminar(int id)
        {
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contrato == null || contrato.FechaTerminacionAnticipada != null)
                return NotFound();

            return View(contrato);
        }

        // POST: Contratos/Terminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Terminar(int id, DateTime fechaTerminacion, decimal? multa, string motivo)
        {
            try
            {
                var contrato = await _context.Contratos.FindAsync(id);
                if (contrato == null || contrato.FechaTerminacionAnticipada != null)
                    return NotFound();

                // Validar que la fecha de terminación sea posterior a la fecha actual
                if (fechaTerminacion <= DateTime.Now)
                {
                    ModelState.AddModelError("", "La fecha de terminación debe ser posterior a la fecha actual.");
                    var contratoCompleto = await _context.Contratos
                        .Include(c => c.Inquilino)
                        .Include(c => c.Inmueble)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    return View(contratoCompleto);
                }

                // Validar que la fecha de terminación sea anterior a la fecha de fin original
                if (fechaTerminacion >= contrato.FechaFin)
                {
                    ModelState.AddModelError("", "La fecha de terminación debe ser anterior a la fecha de fin original.");
                    var contratoCompleto = await _context.Contratos
                        .Include(c => c.Inquilino)
                        .Include(c => c.Inmueble)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    return View(contratoCompleto);
                }

                // Actualizar contrato
                contrato.FechaTerminacionAnticipada = fechaTerminacion;
                contrato.Multa = multa;
                contrato.TerminadoPorUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
                contrato.TerminadoEn = DateTime.Now;

                _context.Update(contrato);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Contrato terminado anticipadamente correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var contrato = await _context.Contratos
                    .Include(c => c.Inquilino)
                    .Include(c => c.Inmueble)
                    .FirstOrDefaultAsync(c => c.Id == id);
                return View(contrato);
            }
        }
    }
}
