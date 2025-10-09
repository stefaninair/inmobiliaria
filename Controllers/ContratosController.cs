using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class ContratosController : Controller
    {
        private readonly RepositorioContrato _repositorio;
        private readonly RepositorioInmueble _repositorioInmueble;
        private readonly RepositorioInquilino _repositorioInquilino;

        public ContratosController(RepositorioContrato repositorio, RepositorioInmueble repositorioInmueble, RepositorioInquilino repositorioInquilino)
        {
            _repositorio = repositorio;
            _repositorioInmueble = repositorioInmueble;
            _repositorioInquilino = repositorioInquilino;
        }

        // GET: Contratos
        public IActionResult Index(int pagina = 1, int tamanoPagina = 5)
        {
            var totalElementos = _repositorio.Contar();
            var contratos = _repositorio.ObtenerPaginados(pagina, tamanoPagina);
            
            var modelo = new PaginacionModel<Contrato>
            {
                Items = contratos,
                PaginaActual = pagina,
                ElementosPorPagina = tamanoPagina,
                TotalElementos = totalElementos,
                TotalPaginas = (int)Math.Ceiling((double)totalElementos / tamanoPagina)
            };
            
            return View(modelo);
        }

        // GET: Contratos/Details/5
        public IActionResult Details(int id)
        {
            var contrato = _repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        // GET: Contratos/Create
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        // POST: Contratos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    contrato.CreadoPorUserId = 1; // TODO: Obtener del usuario actual
                    _repositorio.Alta(contrato);
                    TempData["Success"] = "Contrato creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear el contrato: {ex.Message}";
                }
            }
            CargarListas();
            return View(contrato);
        }

        // GET: Contratos/Edit/5
        public IActionResult Edit(int id)
        {
            var contrato = _repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            CargarListas();
            return View(contrato);
        }

        // POST: Contratos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
            if (id != contrato.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Modificacion(contrato);
                    TempData["Success"] = "Contrato actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar el contrato: {ex.Message}";
                }
            }
            CargarListas();
            return View(contrato);
        }

        // GET: Contratos/Delete/5
        public IActionResult Delete(int id)
        {
            var contrato = _repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        // POST: Contratos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorio.Baja(id);
                TempData["Success"] = "Contrato eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el contrato: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Contratos/Vigentes
        public IActionResult Vigentes()
        {
            var contratos = _repositorio.ObtenerTodos().Where(c => c.Vigente).ToList();
            return View("Index", new PaginacionModel<Contrato>
            {
                Items = contratos,
                PaginaActual = 1,
                ElementosPorPagina = 5,
                TotalElementos = contratos.Count,
                TotalPaginas = 1
            });
        }

        // GET: Contratos/Vencidos
        public IActionResult Vencidos()
        {
            var contratos = _repositorio.ObtenerTodos().Where(c => !c.Vigente).ToList();
            return View("Index", new PaginacionModel<Contrato>
            {
                Items = contratos,
                PaginaActual = 1,
                ElementosPorPagina = 5,
                TotalElementos = contratos.Count,
                TotalPaginas = 1
            });
        }

        // GET: Contratos/PorInmueble
        public IActionResult PorInmueble()
        {
            CargarListas();
            return View();
        }

        // POST: Contratos/PorInmueble
        [HttpPost]
        public IActionResult PorInmueble(int inmuebleId)
        {
            var contratos = _repositorio.ObtenerContratosActivosPorInmueble(inmuebleId);
            CargarListas();
            ViewBag.InmuebleSeleccionado = inmuebleId;
            return View("Index", new PaginacionModel<Contrato>
            {
                Items = contratos,
                PaginaActual = 1,
                ElementosPorPagina = 5,
                TotalElementos = contratos.Count,
                TotalPaginas = 1
            });
        }

        // GET: Contratos/Estadisticas
        public IActionResult Estadisticas()
        {
            var contratos = _repositorio.ObtenerTodos();
            var estadisticas = new
            {
                Total = contratos.Count,
                Vigentes = contratos.Count(c => c.Vigente),
                Vencidos = contratos.Count(c => !c.Vigente),
                MontoTotal = contratos.Sum(c => c.MontoMensual),
                MontoPromedio = contratos.Any() ? contratos.Average(c => c.MontoMensual) : 0
            };
            return View(estadisticas);
        }

        private void CargarListas()
        {
            var inmuebles = _repositorioInmueble.ObtenerDisponibles();
            var inquilinos = _repositorioInquilino.ObtenerTodos();
            
            ViewBag.Inmuebles = inmuebles;
            ViewBag.Inquilinos = inquilinos;
        }
    }
}