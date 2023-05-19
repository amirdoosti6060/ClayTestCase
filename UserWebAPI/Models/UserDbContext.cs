using Microsoft.EntityFrameworkCore;

namespace UserWebAPI.Models
{
    public class UserDbContext: DbContext
    {
        public virtual DbSet<User> Users { get; set; } = null!;

        public UserDbContext(DbContextOptions<UserDbContext> options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {
                entity.ToTable("user");
                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50);
                    
                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.FullName)
                    .HasColumnName("fullname")
                    .HasMaxLength(40);

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasMaxLength(25);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createdAt");
            });
        }
    }
}
