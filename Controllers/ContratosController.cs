using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System;
using System.Linq;

namespace Inmobiliaria.Controllers
{
    public class ContratosController : Controller
    {
        private readonly RepositorioContrato repoContrato;
        private readonly RepositorioInmueble repoInmueble;
        private readonly RepositorioInquilino repoInquilino;

        public ContratosController(RepositorioContrato rc, RepositorioInmueble ri, RepositorioInquilino rq)
        {
            repoContrato = rc;
            repoInmueble = ri;
            repoInquilino = rq;
        }

        private void CargarListas()
        {
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
        }

        // GET: Contratos
        public IActionResult Index()
        {
            var lista = repoContrato.ObtenerTodos();
            ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
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
            try
            {
                if (ModelState.IsValid)
                {
                    repoContrato.Alta(contrato);
                    TempData["Mensaje"] = "Contrato creado correctamente";
                    return RedirectToAction(nameof(Index));
                }

                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errores);
                CargarListas();
                return View(contrato);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                CargarListas();
                return View(contrato);
            }
        }

        // GET: Contratos/Edit/5
        public IActionResult Edit(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            CargarListas();
            return View(contrato);
        }

        // POST: Contratos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
            try
            {
                if (id != contrato.Id)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    repoContrato.Modificacion(contrato);
                    TempData["Mensaje"] = "Contrato modificado correctamente";
                    return RedirectToAction(nameof(Index));
                }

                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errores);
                CargarListas();
                return View(contrato);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                CargarListas();
                return View(contrato);
            }
        }

        // GET: Contratos/Details/5
        public IActionResult Details(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            return View(contrato);
        }

        // GET: Contratos/Delete/5
        public IActionResult Delete(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            return View(contrato);
        }

        // POST: Contratos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repoContrato.Baja(id);
                TempData["Mensaje"] = "Contrato eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                var contrato = repoContrato.ObtenerPorId(id);
                return View(contrato);
            }
        }
    }
}
