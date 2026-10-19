using Microsoft.EntityFrameworkCore;
using MyDoc.Models;

namespace MyDoc.Data;

public sealed class DocSpaceDbContext(DbContextOptions<DocSpaceDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentShare> DocumentShares => Set<DocumentShare>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Document>().HasIndex(x => new { x.OwnerId, x.UpdatedAtUtc });
        modelBuilder.Entity<DocumentShare>().HasIndex(x => new { x.DocumentId, x.UserId }).IsUnique();
        modelBuilder.Entity<DocumentShare>().HasOne(x => x.Document).WithMany(x => x.Shares).HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DocumentShare>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public static class DatabaseSeeder
{
    public static async Task SeedAsync(DocSpaceDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        if (await db.Users.AnyAsync()) return;
        db.Users.AddRange(new User { Name = "Alice", Email = "alice@docspace.local" }, new User { Name = "Bob", Email = "bob@docspace.local" });
        await db.SaveChangesAsync();
    }
}
