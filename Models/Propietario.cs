namespace Inmobiliaria.Models
{
    public class Propietario
    {
        public int Id { get; set; }
        public required string DNI { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
    }
}