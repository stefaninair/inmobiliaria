using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Inquilino> Inquilinos { get; set; }
        public DbSet<TipoInmueble> TiposInmueble { get; set; }
        public DbSet<Inmueble> Inmuebles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones
            modelBuilder.Entity<Inmueble>()
                .HasOne(i => i.Propietario)
                .WithMany()
                .HasForeignKey(i => i.PropietarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inmueble>()
                .HasOne(i => i.TipoInmueble)
                .WithMany()
                .HasForeignKey(i => i.TipoInmuebleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
