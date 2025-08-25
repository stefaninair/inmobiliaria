namespace Inmobiliaria.Models
{
    public class Inquilino
    {
        public int Id { get; set; }
        public required string DNI { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public string? Email { get; set; }
        public required string Telefono { get; set; }
    }
}