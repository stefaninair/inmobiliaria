using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System;

namespace Inmobiliaria.Models
{
    public class RepositorioInquilino
    {
        private readonly ApplicationDbContext _context;

        public RepositorioInquilino(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Inquilino> ObtenerTodos()
        {
            return _context.Inquilinos.ToList();
        }

        public Inquilino? ObtenerPorId(int id)
        {
            return _context.Inquilinos.Find(id);
        }

        public int Alta(Inquilino i)
        {
            _context.Inquilinos.Add(i);
            _context.SaveChanges();
            return i.Id;
        }

        public int Modificacion(Inquilino i)
        {
            _context.Inquilinos.Update(i);
            return _context.SaveChanges();
        }

        public int Baja(int id)
        {
            var i = _context.Inquilinos.Find(id);
            if (i != null)
            {
                _context.Inquilinos.Remove(i);
                return _context.SaveChanges();
            }
            return 0;
        }

        public PaginacionModel<Inquilino> ObtenerPaginados(int pagina = 1, int elementosPorPagina = 5)
        {
            var totalElementos = _context.Inquilinos.Count();
            var offset = (pagina - 1) * elementosPorPagina;
            
            var res = _context.Inquilinos
                .OrderBy(i => i.Id)
                .Skip(offset)
                .Take(elementosPorPagina)
                .ToList();
            
            return new PaginacionModel<Inquilino>(res, pagina, elementosPorPagina, totalElementos);
        }
    }
}
