using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly RepositorioInmueble repoInmueble;
        private readonly RepositorioPropietario repoPropietario;

        public InmueblesController(RepositorioInmueble repoI, RepositorioPropietario repoP)
        {
            this.repoInmueble = repoI;
            this.repoPropietario = repoP;
        }

        // GET: Inmuebles
        public ActionResult Index()
        {
            var lista = repoInmueble.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: Inmuebles/Create
        public ActionResult Create()
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            return View();
        }

        // GET: Inmuebles/Ver/5
        public ActionResult Ver(int id)
        {
            var entidad = id == 0 ? new Inmueble() : repoInmueble.ObtenerPorId(id);
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            return View(entidad);
        }

        // POST: Inmuebles/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Guardar(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (entidad.Id == 0)
                    {
                        repoInmueble.Alta(entidad);
                        TempData["Mensaje"] = "Inmueble creado correctamente";
                    }
                    else
                    {
                        repoInmueble.Modificacion(entidad);
                        TempData["Mensaje"] = "Inmueble modificado correctamente";
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Obtiene los errores de validación y los muestra en el TempData
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = "Errores de validación: " + string.Join(", ", errors);
                    ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                    return View("Create", entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View("Create", entidad);
            }
        }

        // GET: Inmuebles/Eliminar/5
        public ActionResult Eliminar(int id)
        {
            var entidad = repoInmueble.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: Inmuebles/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Inmueble entidad)
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
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }
    }
}


