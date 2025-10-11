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
            
            // Debug: Log del estado de los contratos
            Console.WriteLine("=== ESTADO DE CONTRATOS EN INDEX ===");
            foreach (var contrato in contratos)
            {
                Console.WriteLine($"Contrato {contrato.Id}:");
                Console.WriteLine($"  FechaTerminacionAnticipada: {contrato.FechaTerminacionAnticipada}");
                Console.WriteLine($"  FechaInicio: {contrato.FechaInicio}");
                Console.WriteLine($"  FechaFin: {contrato.FechaFin}");
                Console.WriteLine($"  Vigente: {contrato.Vigente}");
                Console.WriteLine($"  DateTime.Now: {DateTime.Now}");
            }
            
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
            CargarListasParaEdicion(contrato.InmuebleId);
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
            CargarListasParaEdicion(contrato.InmuebleId);
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
            var todosContratos = _repositorio.ObtenerTodos();
            Console.WriteLine($"Total contratos: {todosContratos.Count}");
            
            foreach (var c in todosContratos)
            {
                Console.WriteLine($"Contrato {c.Id}: FechaTerminacionAnticipada={c.FechaTerminacionAnticipada}, Vigente={c.Vigente}");
            }
            
            // Filtro más explícito: solo contratos que NO están terminados anticipadamente Y están vigentes
            var contratos = todosContratos.Where(c => 
                c.FechaTerminacionAnticipada == null && 
                DateTime.Now >= c.FechaInicio && 
                DateTime.Now <= c.FechaFin).ToList();
            
            Console.WriteLine($"Contratos vigentes: {contratos.Count}");
            
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

        // GET: Contratos/PorVencer
        public IActionResult PorVencer(int dias = 30)
        {
            var fechaLimite = DateTime.Now.AddDays(dias);
            var contratos = _repositorio.ObtenerTodos()
                .Where(c => c.FechaTerminacionAnticipada == null && 
                           c.FechaFin >= DateTime.Now && 
                           c.FechaFin <= fechaLimite)
                .OrderBy(c => c.FechaFin)
                .ToList();
            
            ViewBag.Dias = dias;
            return View("Index", new PaginacionModel<Contrato>
            {
                Items = contratos,
                PaginaActual = 1,
                ElementosPorPagina = 5,
                TotalElementos = contratos.Count,
                TotalPaginas = 1
            });
        }

        // GET: Contratos/Pagos/5
        public IActionResult Pagos(int id)
        {
            var contrato = _repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }

            var pagos = _repositorio.ObtenerPagosPorContrato(id);
            ViewBag.Contrato = contrato;
            
            return View(pagos);
        }

        // GET: Contratos/Pagos/Create/5
        public IActionResult CrearPago(int contratoId)
        {
            var contrato = _repositorio.ObtenerPorId(contratoId);
            if (contrato == null)
            {
                return NotFound();
            }

            var pago = new Pago
            {
                ContratoId = contratoId,
                FechaPago = DateTime.Now
            };

            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // POST: Contratos/Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPago(Pago pago)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    pago.CreadoPorUserId = 1; // TODO: Obtener del usuario actual
                    _repositorio.CrearPago(pago);
                    TempData["Success"] = "Pago creado exitosamente.";
                    return RedirectToAction(nameof(Pagos), new { id = pago.ContratoId });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear el pago: {ex.Message}";
                }
            }

            var contrato = _repositorio.ObtenerPorId(pago.ContratoId);
            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // GET: Contratos/Renovar/5
        public IActionResult Renovar(int id)
        {
            var contrato = _repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        // POST: Contratos/Renovar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Renovar(int id, DateTime nuevaFechaFin, decimal? nuevoMontoMensual)
        {
            try
            {
                _repositorio.RenovarContrato(id, nuevaFechaFin, nuevoMontoMensual);
                TempData["Success"] = "Contrato renovado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al renovar el contrato: {ex.Message}";
                var contrato = _repositorio.ObtenerPorId(id);
                return View(contrato);
            }
        }

        // GET: Contratos/Terminar/5
        public IActionResult Terminar(int id)
        {
            var contrato = _repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        // POST: Contratos/Terminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Terminar(int id, DateTime fechaTerminacion, decimal? multa, string? motivo)
        {
            try
            {
                // Debug: Log de los parámetros recibidos
                Console.WriteLine($"Terminando contrato {id}:");
                Console.WriteLine($"Fecha terminación: {fechaTerminacion}");
                Console.WriteLine($"Multa: {multa}");
                Console.WriteLine($"Motivo: {motivo}");
                
                var result = _repositorio.TerminarContrato(id, fechaTerminacion, multa, motivo);
                Console.WriteLine($"Filas afectadas: {result}");
                
                TempData["Success"] = "Contrato terminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al terminar contrato: {ex.Message}");
                TempData["Error"] = $"Error al terminar el contrato: {ex.Message}";
                var contrato = _repositorio.ObtenerPorId(id);
                return View(contrato);
            }
        }

        private void CargarListas()
        {
            var inmuebles = _repositorioInmueble.ObtenerDisponibles();
            var inquilinos = _repositorioInquilino.ObtenerTodos();
            
            ViewBag.Inmuebles = inmuebles;
            ViewBag.Inquilinos = inquilinos;
        }

        private void CargarListasParaEdicion(int inmuebleIdActual)
        {
            // Obtener inmuebles disponibles
            var inmueblesDisponibles = _repositorioInmueble.ObtenerDisponibles();
            
            // Obtener el inmueble actual del contrato
            var inmuebleActual = _repositorioInmueble.ObtenerPorId(inmuebleIdActual);
            
            // Crear lista combinada: inmueble actual + inmuebles disponibles
            var inmuebles = new List<Inmueble>();
            if (inmuebleActual != null)
            {
                inmuebles.Add(inmuebleActual); // Agregar el inmueble actual primero
            }
            
            // Agregar inmuebles disponibles (excluyendo el actual si ya está en disponibles)
            foreach (var inmueble in inmueblesDisponibles)
            {
                if (inmueble.Id != inmuebleIdActual)
                {
                    inmuebles.Add(inmueble);
                }
            }
            
            var inquilinos = _repositorioInquilino.ObtenerTodos();
            
            ViewBag.Inmuebles = inmuebles;
            ViewBag.Inquilinos = inquilinos;
        }
    }
}