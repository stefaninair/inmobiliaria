using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly RepositorioInmueble _repositorio;
        private readonly RepositorioPropietario _repositorioPropietario;
        private readonly RepositorioTipoInmueble _repositorioTipoInmueble;

        public InmueblesController(RepositorioInmueble repositorio, RepositorioPropietario repositorioPropietario, RepositorioTipoInmueble repositorioTipoInmueble)
        {
            _repositorio = repositorio;
            _repositorioPropietario = repositorioPropietario;
            _repositorioTipoInmueble = repositorioTipoInmueble;
        }

        // GET: Inmuebles
        public IActionResult Index(int pagina = 1, int tamanoPagina = 5)
        {
            var totalElementos = _repositorio.Contar();
            var inmuebles = _repositorio.ObtenerPaginados(pagina, tamanoPagina);
            
            var modelo = new PaginacionModel<Inmueble>
            {
                Items = inmuebles,
                PaginaActual = pagina,
                ElementosPorPagina = tamanoPagina,
                TotalElementos = totalElementos,
                TotalPaginas = (int)Math.Ceiling((double)totalElementos / tamanoPagina)
            };
            
            return View(modelo);
        }

        // GET: Inmuebles/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = _repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // GET: Inmuebles/Create
        public IActionResult Create()
        {
            CargarPropietarios();
            CargarTiposInmueble();
            return View();
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Alta(inmueble);
                    TempData["Success"] = "Inmueble creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear el inmueble: {ex.Message}";
                }
            }
            CargarPropietarios();
            CargarTiposInmueble();
            return View(inmueble);
        }

        // GET: Inmuebles/Edit/5
        public IActionResult Edit(int id)
        {
            var inmueble = _repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            CargarPropietarios();
            CargarTiposInmueble();
            return View(inmueble);
        }

        // POST: Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.Modificacion(inmueble);
                    TempData["Success"] = "Inmueble actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar el inmueble: {ex.Message}";
                }
            }
            CargarPropietarios();
            CargarTiposInmueble();
            return View(inmueble);
        }

        // GET: Inmuebles/Delete/5
        public IActionResult Delete(int id)
        {
            var inmueble = _repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: Inmuebles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorio.Baja(id);
                TempData["Success"] = "Inmueble eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el inmueble: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Inmuebles/Disponibles
        public IActionResult Disponibles()
        {
            var inmuebles = _repositorio.ObtenerDisponibles();
            return View(inmuebles);
        }

        private void CargarPropietarios()
        {
            var propietarios = _repositorioPropietario.ObtenerTodos();
            ViewBag.Propietarios = propietarios;
        }

        // GET: Inmuebles/Ocupados
        public IActionResult Ocupados()
        {
            var inmuebles = _repositorio.ObtenerTodos().Where(i => !i.Disponible).ToList();
            return View("Index", new PaginacionModel<Inmueble>
            {
                Items = inmuebles,
                PaginaActual = 1,
                ElementosPorPagina = 5,
                TotalElementos = inmuebles.Count,
                TotalPaginas = 1
            });
        }

        // GET: Inmuebles/Libres
        public IActionResult Libres()
        {
            var inmuebles = _repositorio.ObtenerDisponibles();
            return View("Index", new PaginacionModel<Inmueble>
            {
                Items = inmuebles,
                PaginaActual = 1,
                ElementosPorPagina = 5,
                TotalElementos = inmuebles.Count,
                TotalPaginas = 1
            });
        }

        // GET: Inmuebles/PorPropietario
        public IActionResult PorPropietario()
        {
            CargarPropietarios();
            return View();
        }

        // POST: Inmuebles/PorPropietario
        [HttpPost]
        public IActionResult PorPropietario(int propietarioId)
        {
            var inmuebles = _repositorio.ObtenerPorPropietario(propietarioId);
            CargarPropietarios();
            ViewBag.PropietarioSeleccionado = propietarioId;
            return View("Index", new PaginacionModel<Inmueble>
            {
                Items = inmuebles,
                PaginaActual = 1,
                ElementosPorPagina = 5,
                TotalElementos = inmuebles.Count,
                TotalPaginas = 1
            });
        }

        // GET: Inmuebles/Estadisticas
        public IActionResult Estadisticas()
        {
            var inmuebles = _repositorio.ObtenerTodos();
            var estadisticas = new
            {
                Total = inmuebles.Count,
                Disponibles = inmuebles.Count(i => i.Disponible),
                Ocupados = inmuebles.Count(i => !i.Disponible),
                ValorTotal = inmuebles.Sum(i => i.Precio),
                ValorPromedio = inmuebles.Any() ? inmuebles.Average(i => i.Precio) : 0
            };
            return View(estadisticas);
        }

        private void CargarTiposInmueble()
        {
            var tiposInmueble = _repositorioTipoInmueble.ObtenerTodos();
            ViewBag.TiposInmueble = tiposInmueble;
        }

        // GET: Inmuebles/Imagenes/5
        public ActionResult Imagenes(int id, [FromServices] RepositorioImagen repoImagen)
        {
            var entidad = _repositorio.ObtenerPorId(id);
            if (entidad == null)
                return NotFound();
            entidad.Imagenes = repoImagen.BuscarPorInmueble(id);
            return View(entidad);
        }

        // POST: Inmuebles/Portada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Portada(Imagen entidad, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                //Recuperar el inmueble y eliminar la imagen anterior
                var inmueble = _repositorio.ObtenerPorId(entidad.InmuebleId);
                if (inmueble != null && !string.IsNullOrEmpty(inmueble.Portada))
                {
                    string rutaEliminar = Path.Combine(environment.WebRootPath, inmueble.Portada.TrimStart('/'));
                    if (System.IO.File.Exists(rutaEliminar))
                    {
                        System.IO.File.Delete(rutaEliminar);
                    }
                }
                
                if (entidad.Archivo != null && entidad.Archivo.Length > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = Path.Combine(path, "Inmuebles");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    
                    string fileName = "portada_" + entidad.InmuebleId + Path.GetExtension(entidad.Archivo.FileName);
                    string rutaFisicaCompleta = Path.Combine(path, fileName);
                    
                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        entidad.Archivo.CopyTo(stream);
                    }
                    
                    entidad.Url = "/Uploads/Inmuebles/" + fileName;
                }
                else //sin imagen
                {
                    entidad.Url = string.Empty;
                }
                
                _repositorio.ModificarPortada(entidad.InmuebleId, entidad.Url);
                TempData["Mensaje"] = "Portada actualizada correctamente";
                return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
            }
        }

        // POST: Inmuebles/Disponibles
        [HttpPost]
        public IActionResult Disponibles(DateTime fechaInicio, DateTime fechaFin)
        {
            var inmueblesDisponibles = _repositorio.ObtenerDisponiblesEntreFechas(fechaInicio, fechaFin);
            
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            
            return View("Disponibles", inmueblesDisponibles);
        }

        // GET: Inmuebles/Sincronizar
        public IActionResult Sincronizar()
        {
            try
            {
                var filasActualizadas = _repositorio.SincronizarDisponibilidad();
                TempData["Success"] = $"Sincronización completada. Se actualizaron {filasActualizadas} inmuebles.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al sincronizar: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}