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
		[Display(Name = "Dirección")]
		public string Direccion { get; set; } = "";
		[Required]
		public int Ambientes { get; set; }
		[Required]
		public int Superficie { get; set; }
		public decimal Latitud { get; set; }
		public decimal Longitud { get; set; }
		[Display(Name = "Duenio")]
		public int PropietarioId { get; set; }
		[ForeignKey(nameof(PropietarioId))]
		public Propietario? Duenio { get; set; }
		public string? Portada { get; set; }
		[NotMapped]
		public IFormFile? PortadaFile { get; set; }
		[NotMapped]
		public bool Habilitado { get; set; } = true;
	}
}
