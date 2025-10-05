using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    [Table("Contratos")]
    public class Contrato
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El inquilino es requerido")]
        [Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }
        [ForeignKey(nameof(InquilinoId))]
        public Inquilino? Inquilino { get; set; }

        [Required(ErrorMessage = "El inmueble es requerido")]
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }
        [ForeignKey(nameof(InmuebleId))]
        public Inmueble? Inmueble { get; set; }

        [Required(ErrorMessage = "El monto mensual es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto Mensual")]
        public decimal MontoMensual { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Fecha de Terminación Anticipada")]
        [DataType(DataType.Date)]
        public DateTime? FechaTerminacionAnticipada { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La multa debe ser mayor o igual a 0")]
        [Display(Name = "Multa")]
        public decimal? Multa { get; set; }

        // Campos de auditoría
        [Required]
        [Display(Name = "Creado por")]
        public int CreadoPorUserId { get; set; }
        [ForeignKey(nameof(CreadoPorUserId))]
        public Usuario? CreadoPorUser { get; set; }

        [Required]
        [Display(Name = "Creado en")]
        public DateTime CreadoEn { get; set; }

        [Display(Name = "Terminado por")]
        public int? TerminadoPorUserId { get; set; }
        [ForeignKey(nameof(TerminadoPorUserId))]
        public Usuario? TerminadoPorUser { get; set; }

        [Display(Name = "Terminado en")]
        public DateTime? TerminadoEn { get; set; }

        // Campo legacy para compatibilidad
        [NotMapped]
        public decimal Monto => MontoMensual;

        [NotMapped]
        public bool Vigente => FechaTerminacionAnticipada == null && DateTime.Now >= FechaInicio && DateTime.Now <= FechaFin;
    }
}
