using System;
using JwtAuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthApi.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public virtual DbSet<User>     Users     { get; set; } = null!;
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(
                "Server=MSI;Database=Enterprise;Trusted_Connection=True;TrustServerCertificate=True;"
            );

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ── User ────────────────────────────────────────────────────
    modelBuilder.Entity<User>(entity =>
    {
        entity.ToTable("User");
        entity.HasKey(u => u.UserID);
        entity.Property(u => u.UserID)
              .HasColumnName("UserID")
              .ValueGeneratedNever();
        entity.Property(u => u.UserName)
              .HasColumnName("UserName")
              .HasMaxLength(50)
              .IsUnicode(false)
              .IsRequired();
        entity.Property(u => u.RoleName)
              .HasColumnName("RoleName")
              .HasMaxLength(50)
              .IsUnicode(false)
              .IsRequired();
        entity.Property(u => u.PasswordHash)
              .HasColumnName("PasswordHash")
              .IsRequired();
        entity.Property(u => u.Email)
              .HasColumnName("Email")
              .HasMaxLength(100)
              .IsUnicode(false)
              .IsRequired();
        entity.Property(u => u.IsActive)
              .HasColumnName("IsActive")
              .HasDefaultValue(true);
        entity.Property(u => u.CreatedDate)
              .HasColumnName("CreatedDate")
              .HasDefaultValueSql("sysutcdatetime()");

            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
