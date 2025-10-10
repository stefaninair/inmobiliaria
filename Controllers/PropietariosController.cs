
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Inmobiliaria.Services;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PropietariosController : Controller
    {
        private readonly RepositorioPropietario _repositorio;
        private readonly ValidationService _validationService;

        public PropietariosController(RepositorioPropietario repositorio, ValidationService validationService)
        {
            _repositorio = repositorio;
            _validationService = validationService;
        }

        // GET: Propietarios
        public IActionResult Index(int pagina = 1, int tamanoPagina = 5)
        {
            var totalElementos = _repositorio.Contar();
            var propietarios = _repositorio.ObtenerPaginados(pagina, tamanoPagina);
            
            var modelo = new PaginacionModel<Propietario>
            {
                Items = propietarios,
                PaginaActual = pagina,
                ElementosPorPagina = tamanoPagina,
                TotalElementos = totalElementos,
                TotalPaginas = (int)Math.Ceiling((double)totalElementos / tamanoPagina)
            };
            
            return View(modelo);
        }

        // GET: Propietarios/Details/5
        public IActionResult Details(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // GET: Propietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Alta(propietario);
                    TempData["Success"] = "Propietario creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear el propietario: {ex.Message}";
                }
            }
            return View(propietario);
        }

        // GET: Propietarios/Edit/5
        public IActionResult Edit(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            if (id != propietario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Modificacion(propietario);
                    TempData["Success"] = "Propietario actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar el propietario: {ex.Message}";
                }
            }
            return View(propietario);
        }

        // GET: Propietarios/Delete/5
        public IActionResult Delete(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }

            // Verificar si puede ser eliminado
            var validationResult = _validationService.CanDeletePropietario(id);
            if (!validationResult.IsValid)
            {
                ViewBag.ErrorMessage = validationResult.ErrorMessage;
                ViewBag.ContratosVigentes = _validationService.GetContratosVigentesPropietario(id);
            }

            return View(propietario);
        }

        // POST: Propietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Verificar si puede ser eliminado antes de proceder
            var validationResult = _validationService.CanDeletePropietario(id);
            if (!validationResult.IsValid)
            {
                TempData["Error"] = validationResult.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _repositorio.Baja(id);
                TempData["Success"] = "Propietario eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el propietario: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Propietarios/Buscar/{q}
        [Route("[controller]/Buscar/{q}", Name = "BuscarPropietarios")]
        public IActionResult Buscar(string q)
        {
            try
            {
                var res = _repositorio.BuscarPorNombre(q);
                return Json(new { datos = res });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}