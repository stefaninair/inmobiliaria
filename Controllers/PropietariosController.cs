using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System;
using System.Linq;

namespace Inmobiliaria.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly RepositorioPropietario repositorio;

        public PropietariosController(RepositorioPropietario repo)
        {
            this.repositorio = repo;
        }

        // GET: Propietarios
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: Propietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario p)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(p);
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        // GET: Propietarios/Edit/5
        public IActionResult Edit(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario p)
        {
            if (id != p.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorio.Modificacion(p);
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        // GET: Propietarios/Delete/5
        public IActionResult Delete(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        // POST: Propietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}