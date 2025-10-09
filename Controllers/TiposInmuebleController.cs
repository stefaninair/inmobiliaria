using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class TiposInmuebleController : Controller
    {
        private readonly RepositorioTipoInmueble _repositorio;

        public TiposInmuebleController(RepositorioTipoInmueble repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: TiposInmueble
        public IActionResult Index()
        {
            var tiposInmueble = _repositorio.ObtenerTodos();
            return View(tiposInmueble);
        }

        // GET: TiposInmueble/Details/5
        public IActionResult Details(int id)
        {
            var tipoInmueble = _repositorio.ObtenerPorId(id);
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            return View(tipoInmueble);
        }

        // GET: TiposInmueble/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposInmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoInmueble tipoInmueble)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Alta(tipoInmueble);
                    TempData["Success"] = "Tipo de inmueble creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear el tipo de inmueble: {ex.Message}";
                }
            }
            return View(tipoInmueble);
        }

        // GET: TiposInmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var tipoInmueble = _repositorio.ObtenerPorId(id);
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            return View(tipoInmueble);
        }

        // POST: TiposInmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoInmueble tipoInmueble)
        {
            if (id != tipoInmueble.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Modificacion(tipoInmueble);
                    TempData["Success"] = "Tipo de inmueble actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar el tipo de inmueble: {ex.Message}";
                }
            }
            return View(tipoInmueble);
        }

        // GET: TiposInmueble/Delete/5
        public IActionResult Delete(int id)
        {
            var tipoInmueble = _repositorio.ObtenerPorId(id);
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            return View(tipoInmueble);
        }

        // POST: TiposInmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorio.Baja(id);
                TempData["Success"] = "Tipo de inmueble eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el tipo de inmueble: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}