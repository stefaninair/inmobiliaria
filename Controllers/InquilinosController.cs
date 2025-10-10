using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Inmobiliaria.Services;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class InquilinosController : Controller
    {
        private readonly RepositorioInquilino _repositorio;
        private readonly ValidationService _validationService;

        public InquilinosController(RepositorioInquilino repositorio, ValidationService validationService)
        {
            _repositorio = repositorio;
            _validationService = validationService;
        }

        // GET: Inquilinos
        public IActionResult Index(int pagina = 1, int tamanoPagina = 5)
        {
            var totalElementos = _repositorio.Contar();
            var inquilinos = _repositorio.ObtenerPaginados(pagina, tamanoPagina);
            
            var modelo = new PaginacionModel<Inquilino>
            {
                Items = inquilinos,
                PaginaActual = pagina,
                ElementosPorPagina = tamanoPagina,
                TotalElementos = totalElementos,
                TotalPaginas = (int)Math.Ceiling((double)totalElementos / tamanoPagina)
            };
            
            return View(modelo);
        }

        // GET: Inquilinos/Details/5
        public IActionResult Details(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Alta(inquilino);
                    TempData["Success"] = "Inquilino creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear el inquilino: {ex.Message}";
                }
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Edit/5
        public IActionResult Edit(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            if (id != inquilino.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Modificacion(inquilino);
                    TempData["Success"] = "Inquilino actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar el inquilino: {ex.Message}";
                }
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Delete/5
        public IActionResult Delete(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }

            // Verificar si puede ser eliminado
            var validationResult = _validationService.CanDeleteInquilino(id);
            if (!validationResult.IsValid)
            {
                ViewBag.ErrorMessage = validationResult.ErrorMessage;
                ViewBag.ContratosVigentes = _validationService.GetContratosVigentesInquilino(id);
            }

            return View(inquilino);
        }

        // POST: Inquilinos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Verificar si puede ser eliminado antes de proceder
            var validationResult = _validationService.CanDeleteInquilino(id);
            if (!validationResult.IsValid)
            {
                TempData["Error"] = validationResult.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _repositorio.Baja(id);
                TempData["Success"] = "Inquilino eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el inquilino: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Inquilinos/Buscar/{q}
        [Route("[controller]/Buscar/{q}", Name = "BuscarInquilinos")]
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