using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    public enum Rol
    {
        Administrador = 1,
        Empleado = 2
    }

    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La clave es requerida")]
        [StringLength(255, ErrorMessage = "La clave no puede exceder los 255 caracteres")]
        [Display(Name = "Clave")]
        public string ClaveHash { get; set; } = "";

        [Required(ErrorMessage = "El rol es requerido")]
        [Display(Name = "Rol")]
        public Rol Rol { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = "";

        [StringLength(200, ErrorMessage = "La ruta del avatar no puede exceder los 200 caracteres")]
        [Display(Name = "Avatar")]
        public string? AvatarPath { get; set; }

        [NotMapped]
        [Display(Name = "Avatar")]
        public IFormFile? AvatarFile { get; set; }

        [NotMapped]
        [Display(Name = "Clave Actual")]
        [DataType(DataType.Password)]
        public string? ClaveActual { get; set; }

        [NotMapped]
        [Display(Name = "Nueva Clave")]
        [DataType(DataType.Password)]
        public string? NuevaClave { get; set; }

        [NotMapped]
        [Display(Name = "Confirmar Clave")]
        [DataType(DataType.Password)]
        public string? ConfirmarClave { get; set; }

        [NotMapped]
        public string RolNombre => Rol switch
        {
            Rol.Administrador => "Administrador",
            Rol.Empleado => "Empleado",
            _ => "Desconocido"
        };
    }
}
