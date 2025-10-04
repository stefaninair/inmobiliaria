using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Hosting;

namespace Inmobiliaria.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly RepositorioInmueble repoInmueble;
        private readonly RepositorioPropietario repoPropietario;
        private readonly IWebHostEnvironment env;

        public InmueblesController(RepositorioInmueble repoI, RepositorioPropietario repoP, IWebHostEnvironment environment)
        {
            repoInmueble = repoI;
            repoPropietario = repoP;
            env = environment;
        }

        private void CargarPropietarios()
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
        }

        // GET: Inmuebles
        public IActionResult Index()
        {
            var lista = repoInmueble.ObtenerTodos();
            ViewBag.Id = TempData["Id"];
            ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: Inmuebles/Create
        public IActionResult Create()
        {
            CargarPropietarios();
            return View();
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (entidad.PortadaFile != null)
                    {
                        string ruta = Path.Combine(env.WebRootPath, "img", entidad.PortadaFile.FileName);
                        using (var stream = new FileStream(ruta, FileMode.Create))
                        {
                            entidad.PortadaFile.CopyTo(stream);
                        }
                        entidad.Portada = entidad.PortadaFile.FileName;
                    }

                    repoInmueble.Alta(entidad);
                    TempData["Mensaje"] = "Inmueble creado correctamente";
                    return RedirectToAction(nameof(Index));
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errors);
                CargarPropietarios();
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                CargarPropietarios();
                return View(entidad);
            }
        }

        // GET: Inmuebles/Edit/5
        public IActionResult Edit(int id)
        {
            var entidad = repoInmueble.ObtenerPorId(id);
            if (entidad == null)
                return NotFound();

            CargarPropietarios();
            return View(entidad);
        }

        // POST: Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble entidad)
        {
            try
            {
                if (id != entidad.Id)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    if (entidad.PortadaFile != null)
                    {
                        string ruta = Path.Combine(env.WebRootPath, "img", entidad.PortadaFile.FileName);
                        using (var stream = new FileStream(ruta, FileMode.Create))
                        {
                            entidad.PortadaFile.CopyTo(stream);
                        }
                        entidad.Portada = entidad.PortadaFile.FileName;
                    }

                    repoInmueble.Modificacion(entidad);
                    TempData["Mensaje"] = "Inmueble modificado correctamente";
                    return RedirectToAction(nameof(Index));
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errors);
                CargarPropietarios();
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                CargarPropietarios();
                return View(entidad);
            }
        }

        // GET: Inmuebles/Details/5
        public IActionResult Details(int id)
        {
            var entidad = repoInmueble.ObtenerPorId(id);
            if (entidad == null)
                return NotFound();

            return View(entidad);
        }

        // GET: Inmuebles/Delete/5
        public IActionResult Delete(int id)
        {
            var entidad = repoInmueble.ObtenerPorId(id);
            if (entidad == null)
                return NotFound();

            ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: Inmuebles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repoInmueble.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                var entidad = repoInmueble.ObtenerPorId(id);
                return View(entidad);
            }
        }
    }
}
