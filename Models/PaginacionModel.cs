using System.Collections.Generic;

namespace Inmobiliaria.Models
{
    public class PaginacionModel<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalElementos { get; set; }
        public int ElementosPorPagina { get; set; } = 5;
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
        public int PaginaAnterior => PaginaActual - 1;
        public int PaginaSiguiente => PaginaActual + 1;

        public PaginacionModel()
        {
        }

        public PaginacionModel(List<T> items, int paginaActual, int elementosPorPagina, int totalElementos)
        {
            Items = items;
            PaginaActual = paginaActual;
            ElementosPorPagina = elementosPorPagina;
            TotalElementos = totalElementos;
            TotalPaginas = (int)Math.Ceiling((double)totalElementos / elementosPorPagina);
        }
    }
}
