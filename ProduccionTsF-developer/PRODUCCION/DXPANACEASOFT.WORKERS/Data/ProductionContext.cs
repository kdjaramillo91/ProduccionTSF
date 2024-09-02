using Microsoft.EntityFrameworkCore;

namespace DXPANACEASOFT.WORKERS
{
    public partial class ProductionContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ProductionContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ProductionContext(DbContextOptions<ProductionContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<BackgroundProcess> BackgroundProcesses { get; set; } = null!;
        public virtual DbSet<MonthlyBalance> MonthlyBalances { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ProductionDatabase"),
                    opts => opts.CommandTimeout(
                        (int)TimeSpan.FromMinutes(_configuration.GetValue<int>("TimeoutDB")).TotalSeconds)
                    );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BackgroundProcess>(entity =>
            {
                entity.ToTable("BackgroundProcess");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.DateCreation)
                    .HasColumnType("datetime")
                    .HasColumnName("dateCreation");

                entity.Property(e => e.DateModification)
                    .HasColumnType("datetime")
                    .HasColumnName("dateModification");

                entity.Property(e => e.State)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("state");
            });

            modelBuilder.Entity<MonthlyBalance>(entity =>
            {
                entity.ToTable("MonthlyBalance");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CodeMetricUnit)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("code_metric_unit");

                entity.Property(e => e.Entrada).HasColumnType("decimal(20, 6)");

                entity.Property(e => e.IdItem).HasColumnName("id_item");

                entity.Property(e => e.IdMetricUnit).HasColumnName("id_metric_unit");

                entity.Property(e => e.IdPresentation).HasColumnName("id_presentation");

                entity.Property(e => e.IdWarehouse).HasColumnName("id_warehouse");

                entity.Property(e => e.LbEntrada)
                    .HasColumnType("decimal(20, 6)")
                    .HasColumnName("LB_Entrada");

                entity.Property(e => e.LbSaldoActual)
                    .HasColumnType("decimal(20, 6)")
                    .HasColumnName("LB_SaldoActual");

                entity.Property(e => e.LbSaldoAnterior)
                    .HasColumnType("decimal(20, 6)")
                    .HasColumnName("LB_SaldoAnterior");

                entity.Property(e => e.LbSalida)
                    .HasColumnType("decimal(20, 6)")
                    .HasColumnName("LB_Salida");

                entity.Property(e => e.Maximum)
                    .HasColumnType("decimal(20, 6)")
                    .HasColumnName("maximum");

                entity.Property(e => e.Minimum)
                    .HasColumnType("decimal(20, 6)")
                    .HasColumnName("minimum");

                entity.Property(e => e.NameItem)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("name_item");

                entity.Property(e => e.NameMetricUnit)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("name_metric_unit");

                entity.Property(e => e.NamePresentation)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("name_presentation");

                entity.Property(e => e.NameWarehouse)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("name_warehouse");

                entity.Property(e => e.SaldoActual).HasColumnType("decimal(20, 6)");

                entity.Property(e => e.SaldoAnterior).HasColumnType("decimal(20, 6)");

                entity.Property(e => e.Salida).HasColumnType("decimal(20, 6)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
