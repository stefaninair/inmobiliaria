using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using System.Linq;

namespace Inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly InmobiliariaContext _context;

        public InquilinoController(InmobiliariaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var lista = _context.Inquilino.ToList();
            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _context.Inquilino.Add(inquilino);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquilino = _context.Inquilino.FirstOrDefault(m => m.Id == id);
            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var inquilino = _context.Inquilino.Find(id);
            if (inquilino != null)
            {
                _context.Inquilino.Remove(inquilino);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
