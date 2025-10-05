using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class TiposInmuebleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TiposInmuebleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TiposInmueble
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposInmueble.ToListAsync());
        }

        // GET: TiposInmueble/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInmueble = await _context.TiposInmueble
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoInmueble == null)
            {
                return NotFound();
            }

            return View(tipoInmueble);
        }

        // GET: TiposInmueble/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposInmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] TipoInmueble tipoInmueble)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoInmueble);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tipo de inmueble creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(tipoInmueble);
        }

        // GET: TiposInmueble/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInmueble = await _context.TiposInmueble.FindAsync(id);
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            return View(tipoInmueble);
        }

        // POST: TiposInmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] TipoInmueble tipoInmueble)
        {
            if (id != tipoInmueble.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoInmueble);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Tipo de inmueble actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoInmuebleExists(tipoInmueble.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipoInmueble);
        }

        // GET: TiposInmueble/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInmueble = await _context.TiposInmueble
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoInmueble == null)
            {
                return NotFound();
            }

            return View(tipoInmueble);
        }

        // POST: TiposInmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoInmueble = await _context.TiposInmueble.FindAsync(id);
            if (tipoInmueble != null)
            {
                _context.TiposInmueble.Remove(tipoInmueble);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tipo de inmueble eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TipoInmuebleExists(int id)
        {
            return _context.TiposInmueble.Any(e => e.Id == id);
        }
    }
}
