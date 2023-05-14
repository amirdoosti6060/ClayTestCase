﻿using Microsoft.EntityFrameworkCore;

namespace DoorWebAPI.Models
{
    public class DoorDbContext: DbContext
    {
        public virtual DbSet<Door> DoorSet { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;

        public DoorDbContext(DbContextOptions<DoorDbContext> options): base (options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Door>(entity =>
            {
                entity.ToTable("door");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.State)
                    .HasColumnName("state");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnName("modifiedAt");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permission");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.DoorId)
                    .HasColumnName("doorId");

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasMaxLength(100);
            });
        }
    }
}
