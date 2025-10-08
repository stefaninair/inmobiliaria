using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;

namespace Inmobiliaria.Models
{
    public class RepositorioPropietario
    {
        private readonly ApplicationDbContext _context;

        public RepositorioPropietario(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Propietario> ObtenerTodos()
        {
            return _context.Propietarios.ToList();
        }

        public Propietario? ObtenerPorId(int id)
        {
            return _context.Propietarios.Find(id);
        }

        public int Alta(Propietario p)
        {
            _context.Propietarios.Add(p);
            _context.SaveChanges();
            return p.Id;
        }

        public int Modificacion(Propietario p)
        {
            _context.Propietarios.Update(p);
            return _context.SaveChanges();
        }

        public int Baja(int id)
        {
            var p = _context.Propietarios.Find(id);
            if (p != null)
            {
                _context.Propietarios.Remove(p);
                return _context.SaveChanges();
            }
            return 0;
        }

        public PaginacionModel<Propietario> ObtenerPaginados(int pagina = 1, int elementosPorPagina = 5)
        {
            var totalElementos = _context.Propietarios.Count();
            var offset = (pagina - 1) * elementosPorPagina;
            
            var res = _context.Propietarios
                .OrderBy(p => p.Id)
                .Skip(offset)
                .Take(elementosPorPagina)
                .ToList();
            
            return new PaginacionModel<Propietario>(res, pagina, elementosPorPagina, totalElementos);
        }
    }
}