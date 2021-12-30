using Microsoft.EntityFrameworkCore;

namespace GroceryListApi.Database;

public class GroceryListDb : DbContext
{
    public GroceryListDb(DbContextOptions<GroceryListDb> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username, "IX_Username");

        modelBuilder.Entity<User>()
            .HasMany(u => u.Stores)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Items)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId);

        modelBuilder.Entity<GroceryStore>()
            .HasMany(s => s.Items)
            .WithOne(i => i.Store)
            .HasForeignKey(i => i.StoreId);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<GroceryStore> Stores => Set<GroceryStore>();
    public DbSet<GroceryListItem> Items => Set<GroceryListItem>();
}