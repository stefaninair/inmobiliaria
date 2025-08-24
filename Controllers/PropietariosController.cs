using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Data; // Asegúrate de que este sea el namespace de tu contexto
using Inmobiliaria.Models;

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
    }
}