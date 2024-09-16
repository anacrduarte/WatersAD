using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using WatersAD.Data.Entities;

namespace WatersAD.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) 
        {
            
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Locality> Localities { get; set; }

        public DbSet<WaterMeter> WaterMeters { get; set; }

        public DbSet<WaterMeterService> WaterMeterServices{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Ver se faz sentido
            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }


            modelBuilder.Entity<Client>().HasIndex(c => c.NIF).IsUnique();
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<City>().HasIndex(c => c.Name).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
