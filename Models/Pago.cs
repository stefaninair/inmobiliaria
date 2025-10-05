using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    [Table("Pagos")]
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El contrato es requerido")]
        [Display(Name = "Contrato")]
        public int ContratoId { get; set; }
        [ForeignKey(nameof(ContratoId))]
        public Contrato? Contrato { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La fecha de pago es requerida")]
        [Display(Name = "Fecha de Pago")]
        [DataType(DataType.Date)]
        public DateTime FechaPago { get; set; }

        [Required(ErrorMessage = "El período es requerido")]
        [Display(Name = "Período")]
        [StringLength(20, ErrorMessage = "El período no puede exceder los 20 caracteres")]
        public string Periodo { get; set; } = ""; // Formato: "2024-01", "2024-02", etc.

        [Display(Name = "Observaciones")]
        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
        public string? Observaciones { get; set; }

        // Campos de auditoría
        [Required]
        [Display(Name = "Creado por")]
        public int CreadoPorUserId { get; set; }
        [ForeignKey(nameof(CreadoPorUserId))]
        public Usuario? CreadoPorUser { get; set; }

        [Required]
        [Display(Name = "Creado en")]
        public DateTime CreadoEn { get; set; }

        [Display(Name = "Anulado por")]
        public int? AnuladoPorUserId { get; set; }
        [ForeignKey(nameof(AnuladoPorUserId))]
        public Usuario? AnuladoPorUser { get; set; }

        [Display(Name = "Anulado en")]
        public DateTime? AnuladoEn { get; set; }

        [Display(Name = "Motivo de Anulación")]
        [StringLength(200, ErrorMessage = "El motivo no puede exceder los 200 caracteres")]
        public string? MotivoAnulacion { get; set; }

        // Soft delete
        [Display(Name = "Eliminado")]
        public bool Eliminado { get; set; } = false;

        [Display(Name = "Eliminado por")]
        public int? EliminadoPorUserId { get; set; }
        [ForeignKey(nameof(EliminadoPorUserId))]
        public Usuario? EliminadoPorUser { get; set; }

        [Display(Name = "Eliminado en")]
        public DateTime? EliminadoEn { get; set; }

        // Propiedades calculadas
        [NotMapped]
        public bool Anulado => AnuladoEn != null;

        [NotMapped]
        public bool Activo => !Eliminado && !Anulado;

        [NotMapped]
        public string Estado
        {
            get
            {
                if (Eliminado) return "Eliminado";
                if (Anulado) return "Anulado";
                return "Activo";
            }
        }
    }
}

