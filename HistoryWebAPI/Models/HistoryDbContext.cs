using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HistoryWebAPI.Models
{
    public class HistoryDbContext: DbContext
    {
        public DbSet<History> History { get; set; } = null!;

        public HistoryDbContext(DbContextOptions<HistoryDbContext> options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Database.SetInitializer<HistoryDbContext>(null);
            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("history");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.DoorId)
                    .HasColumnName("doorId");

                entity.Property(e => e.DoorName)
                    .HasColumnName("doorName")
                    .HasMaxLength(100);

                entity.Property(e => e.HardwareId)
                    .HasColumnName("hardwareId")
                    .HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .HasColumnName("userId");

                entity.Property(e => e.FullName)
                    .HasColumnName("fullname")
                    .HasMaxLength(150);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasMaxLength(100);

                entity.Property(e => e.ActionStatus)
                    .HasColumnName("actionStatus")
                    .HasMaxLength(50);

                entity.Property(e => e.TimeStamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");
            });
        }
    }
}
