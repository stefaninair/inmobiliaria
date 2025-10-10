using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria.Models
{
	[Table("Inmuebles")]
	public class Inmueble
	{
		[Display(Name = "Nº")]
		public int Id { get; set; }

		[Required(ErrorMessage = "La dirección es requerida")]
		[StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
		[Display(Name = "Dirección")]
		public string Direccion { get; set; } = "";

		[Required(ErrorMessage = "El uso es requerido")]
		[StringLength(50, ErrorMessage = "El uso no puede exceder los 50 caracteres")]
		[Display(Name = "Uso")]
		public string Uso { get; set; } = "";

		[Required(ErrorMessage = "La cantidad de ambientes es requerida")]
		[Range(1, 20, ErrorMessage = "Los ambientes deben estar entre 1 y 20")]
		[Display(Name = "Ambientes")]
		public int Ambientes { get; set; }

		[Range(1, 10000, ErrorMessage = "La superficie debe estar entre 1 y 10000 m²")]
		[Display(Name = "Superficie (m²)")]
		public decimal? Superficie { get; set; }

		[Required(ErrorMessage = "El precio es requerido")]
		[Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
		[Display(Name = "Precio")]
		public decimal Precio { get; set; }

		[Display(Name = "Disponible")]
		public bool Disponible { get; set; } = true;

		[Required(ErrorMessage = "El propietario es requerido")]
		[Display(Name = "Propietario")]
		public int PropietarioId { get; set; }
		[ForeignKey(nameof(PropietarioId))]
		public Propietario? Propietario { get; set; }

		[Required(ErrorMessage = "El tipo de inmueble es requerido")]
		[Display(Name = "Tipo de Inmueble")]
		public int TipoInmuebleId { get; set; }
		[ForeignKey(nameof(TipoInmuebleId))]
		public TipoInmueble? TipoInmueble { get; set; }

		[StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
		[Display(Name = "Observaciones")]
		public string? Observaciones { get; set; }

		// Campos legacy para compatibilidad
		public decimal Latitud { get; set; }
		public decimal Longitud { get; set; }
		public string? Portada { get; set; }
		[NotMapped]//Para EF
		public IFormFile? PortadaFile { get; set; }
		[ForeignKey(nameof(Imagen.InmuebleId))]
		public IList<Imagen> Imagenes { get; set; } = new List<Imagen>();
		public bool Habilitado { get; set; } = true;
	}
}
