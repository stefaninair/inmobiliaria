using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
    {
        public class Propietario
        {
            [Key]
            [Display(Name = "Código")]
            public int Id { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; } = "";

            [Required]
            [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios")]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; } = "";

            [Required]
            [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe contener solo números y tener entre 7 y 8 dígitos")]
            [Display(Name = "DNI")]
            public string Dni { get; set; } = "";

            [Display(Name = "Teléfono")]
            [RegularExpression(@"^\d{10,15}$", ErrorMessage = "El teléfono debe contener solo números y tener entre 10 y 15 dígitos")]
            public string? Telefono { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; } = "";
            
            [Required, DataType(DataType.Password)]
            [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "La clave solo puede contener letras y números, sin símbolos")]
            [Display(Name = "Clave")]
            public string Clave { get; set; } = "";
        }
    }
    