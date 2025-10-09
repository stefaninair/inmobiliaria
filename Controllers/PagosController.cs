using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Inmobiliaria.Services;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly PagoService _pagoService;
        private readonly RepositorioContrato _repositorioContrato;

        public PagosController(PagoService pagoService, RepositorioContrato repositorioContrato)
        {
            _pagoService = pagoService;
            _repositorioContrato = repositorioContrato;
        }

        // GET: Pagos
        public IActionResult Index()
        {
            var pagos = _pagoService.ObtenerPagosActivosAsync().Result;
            return View(pagos);
        }

        // GET: Pagos/Details/5
        public IActionResult Details(int id)
        {
            var pagos = _pagoService.ObtenerPagosActivosAsync().Result;
            var pago = pagos.FirstOrDefault(p => p.Id == id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // GET: Pagos/Create
        public IActionResult Create()
        {
            var contratos = _repositorioContrato.ObtenerTodos();
            ViewBag.Contratos = contratos;
            return View();
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _pagoService.CrearPagoAsync(pago).Wait();
                    TempData["Success"] = "Pago creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear el pago: {ex.Message}";
                }
            }
            var contratos = _repositorioContrato.ObtenerTodos();
            ViewBag.Contratos = contratos;
            return View(pago);
        }

        // GET: Pagos/Edit/5
        public IActionResult Edit(int id)
        {
            var pagos = _pagoService.ObtenerPagosActivosAsync().Result;
            var pago = pagos.FirstOrDefault(p => p.Id == id);
            if (pago == null)
            {
                return NotFound();
            }
            var contratos = _repositorioContrato.ObtenerTodos();
            ViewBag.Contratos = contratos;
            return View(pago);
        }

        // POST: Pagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Implementar actualización de pago
                    TempData["Success"] = "Pago actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar el pago: {ex.Message}";
                }
            }
            var contratos = _repositorioContrato.ObtenerTodos();
            ViewBag.Contratos = contratos;
            return View(pago);
        }

        // GET: Pagos/Delete/5
        public IActionResult Delete(int id)
        {
            var pagos = _pagoService.ObtenerPagosActivosAsync().Result;
            var pago = pagos.FirstOrDefault(p => p.Id == id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _pagoService.EliminarPagoAsync(id).Wait();
                TempData["Success"] = "Pago eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el pago: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Pagos/PorContrato/5
        public IActionResult PorContrato(int contratoId)
        {
            var pagos = _pagoService.ObtenerPagosPorContratoAsync(contratoId).Result;
            return View(pagos);
        }

        // GET: Pagos/Anular/5
        public IActionResult Anular(int id)
        {
            var pagos = _pagoService.ObtenerPagosActivosAsync().Result;
            var pago = pagos.FirstOrDefault(p => p.Id == id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // POST: Pagos/Anular/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Anular(int id, string motivo)
        {
            try
            {
                _pagoService.AnularPagoAsync(id, motivo).Wait();
                TempData["Success"] = "Pago anulado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al anular el pago: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}