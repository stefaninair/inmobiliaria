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
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Pago> Pagos { get; set; }

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

            // Configuración de relaciones para Contrato
            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Inquilino)
                .WithMany()
                .HasForeignKey(c => c.InquilinoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Inmueble)
                .WithMany()
                .HasForeignKey(c => c.InmuebleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.CreadoPorUser)
                .WithMany()
                .HasForeignKey(c => c.CreadoPorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.TerminadoPorUser)
                .WithMany()
                .HasForeignKey(c => c.TerminadoPorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de relaciones para Pago
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Contrato)
                .WithMany()
                .HasForeignKey(p => p.ContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.CreadoPorUser)
                .WithMany()
                .HasForeignKey(p => p.CreadoPorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.AnuladoPorUser)
                .WithMany()
                .HasForeignKey(p => p.AnuladoPorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.EliminadoPorUser)
                .WithMany()
                .HasForeignKey(p => p.EliminadoPorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
