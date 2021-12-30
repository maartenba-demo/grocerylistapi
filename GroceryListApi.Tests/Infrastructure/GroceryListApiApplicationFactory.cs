using System;
using System.Linq;
using GroceryListApi.Database;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GroceryListApi.Tests.Infrastructure;

[UsedImplicitly]
public class GroceryListApiApplicationFactory : WebApplicationFactory<Program>
{
    private readonly object _dbLock = new();
    private readonly string _dbName = Guid.NewGuid().ToString();
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Set in-memory database
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<GroceryListDb>));
            services.Remove(descriptor);

            services.AddDbContext<GroceryListDb>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
 
        var app = base.CreateHost(builder);

        // Seed data
        using var scope = app.Services.CreateScope();
        using var db = scope.ServiceProvider.GetService<GroceryListDb>()!;
        if (!db.Users.Any())
        {
            lock (_dbLock) 
            {
                if (!db.Users.Any())
                {
                    db.Users.Add(new User { Id = 1, Username = "test", Name = "Test", Email = "test@example.com" });
                    db.Stores.Add(new GroceryStore { Id = 1, UserId = 1, Name = "Test", Description = "Test store 1" });
                    db.Stores.Add(new GroceryStore { Id = 2, UserId = 1, Name = "Test", Description = "Test store 2" });
                    db.Items.Add(new GroceryListItem { Id = 1, UserId = 1, StoreId = 1, Title = "Test 1-1", IsComplete = true });
                    db.Items.Add(new GroceryListItem { Id = 2, UserId = 1, StoreId = 1, Title = "Test 1-2", IsComplete = false });
                    db.Items.Add(new GroceryListItem { Id = 3, UserId = 1, StoreId = 2, Title = "Test 2-1", IsComplete = true });
                    db.SaveChanges();
                }
            }
        }

        return app;
    }
}