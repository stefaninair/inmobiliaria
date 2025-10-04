using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    [Table("Contratos")]
    public class Contrato
    {
        public int Id { get; set; }

        [Required]
        public int InmuebleId { get; set; }

        [ForeignKey(nameof(InmuebleId))]
        public Inmueble? Inmueble { get; set; }

        [Required]
        public int InquilinoId { get; set; }

        [ForeignKey(nameof(InquilinoId))]
        public Inquilino? Inquilino { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Display(Name = "Fecha Fin")]
        public DateTime FechaFin { get; set; }

        [Required]
        public decimal Monto { get; set; }

        public bool Vigente { get; set; } = true;
    }
}
