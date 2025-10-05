using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class TipoInmueble
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        [Display(Name = "Tipo de Inmueble")]
        public string Nombre { get; set; } = "";
    }
}
