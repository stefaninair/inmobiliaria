using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria.Models;
using System;
using System.Linq;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class InquilinosController : Controller
    {
        private readonly RepositorioInquilino repositorio;

        public InquilinosController(RepositorioInquilino repo)
        {
            this.repositorio = repo;
        }

        // GET: Inquilinos
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: Inquilinos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino i)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(i);
                return RedirectToAction(nameof(Index));
            }
            return View(i);
        }

        // GET: Inquilinos/Edit/5
        public IActionResult Edit(int id)
        {
            var i = repositorio.ObtenerPorId(id);
            if (i == null)
            {
                return NotFound();
            }
            return View(i);
        }

        // POST: Inquilinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino i)
        {
            if (id != i.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorio.Modificacion(i);
                return RedirectToAction(nameof(Index));
            }
            return View(i);
        }

        // GET: Inquilinos/Delete/5
        [Authorize(Policy = "SoloAdmin")]
        public IActionResult Delete(int id)
        {
            var i = repositorio.ObtenerPorId(id);
            if (i == null)
            {
                return NotFound();
            }
            return View(i);
        }

        // POST: Inquilinos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SoloAdmin")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            TempData["Success"] = "Inquilino eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
