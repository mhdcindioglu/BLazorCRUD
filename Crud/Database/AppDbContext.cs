using Crud.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crud.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Title).IsUnique();
            entity.Property(e => e.Title).HasMaxLength(128).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(16,4)");
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Title).IsUnique();
            entity.Property(e => e.Title).HasMaxLength(64).IsRequired();
        });
    }
}