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

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasMaxLength(100);

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasComment("0 = not activated, 1 = activated, 2 = disabled");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createdAt");
            });
        }
    }
}
