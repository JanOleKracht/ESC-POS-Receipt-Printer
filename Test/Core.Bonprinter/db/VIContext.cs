using Microsoft.EntityFrameworkCore;

namespace Core.Bonprinter.db
{
    public class VIContext : DbContext
    {
        public VIContext() : base()
        {
        }

        //public VIContext(DbContextOptions<VIContext> options) : base(options)
        //{
        //}

        public DbSet<BelegView> Belege { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source =(local)\\SQLEXPRESS02; Initial Catalog = rsm; Integrated Security=true;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BelegView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vPatientBeleg");
                entity.Property(x => x.IstZuzahlung).HasConversion(v => v ? 1 : 0, v => v == 1);
            });
        }
    }
}