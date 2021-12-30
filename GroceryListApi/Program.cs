using GroceryListApi.Endpoints;
using GroceryListApi.Startup;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Setup services
builder.Services.AddSqlite<GroceryListDb>("Data Source=grocerylists.db;Cache=Shared");
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.AddAuthenticationServices();
builder.AddSwaggerServices();

// Setup application
var app = builder.Build();

await EnsureDb(app.Services, app.Logger);

app.MapSwaggerEndpoints();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticationEndpoints();
app.MapGroceryStoreEndpoints();
app.MapGroceryListItemEndpoints();

app.Run();

async Task EnsureDb(IServiceProvider services, ILogger logger)
{
    await using var db = services.CreateScope().ServiceProvider.GetRequiredService<GroceryListDb>();
    if (db.Database.IsRelational())
    {
        logger.LogInformation("Updating database...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Updated database");
    }
}

public partial class Program { }