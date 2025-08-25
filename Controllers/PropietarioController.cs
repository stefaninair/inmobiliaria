using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Data; // Asegúrate de que este sea el namespace de tu contexto
using Inmobiliaria.Models;
using System.Linq;

namespace Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly InmobiliariaContext _context;

        public PropietarioController(InmobiliariaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Obtiene la lista de propietario de la base de datos
            var lista = _context.Propietario.ToList();

            // Pasa la lista de propietario a la vista
            return View(lista);
        }
        //Crear
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                _context.Propietario.Add(propietario);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }
        //Eliminar
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propietario = _context.Propietario.FirstOrDefault(m => m.Id == id);
            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var propietario = _context.Propietario.Find(id);
            if (propietario != null)
            {
                _context.Propietario.Remove(propietario);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        //Modificar

        public IActionResult Edit(int id)
        {
            var propietario = _context.Propietario.Find(id);
            if (propietario == null)
            {
                return NotFound(); // Esto se ejecuta si el ID no existe en la base de datos
            }
            return View(propietario); // Esto envía el objeto a la vista Edit.cshtml
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            if (id != propietario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propietario);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Propietario.Any(e => e.Id == id))
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
            return View(propietario);
        }

    }
}