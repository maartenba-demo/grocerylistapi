using System.Security.Claims;
using GroceryListApi.Endpoints.Schemas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
// ReSharper disable RedundantArgumentDefaultValue

namespace GroceryListApi.Endpoints;

public static class GroceryStoreEndpoints
{
    private const string Tag = "Stores";
    private const string ById = "stores/byid";

    public static WebApplication MapGroceryStoreEndpoints(this WebApplication app)
    {
        app.MapGet("/stores", GetAllStores)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<ICollection<ApiStore>>(200)
            .Produces(401)
            .WithDisplayName("Get all stores");
        
        app.MapGet("/stores/{storeId}", GetStore)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<ApiStore>(200)
            .Produces(404)
            .Produces(401)
            .WithName(ById)
            .WithDisplayName("Get a store by id");
        
        app.MapDelete("/stores/{storeId}", DeleteStore)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Delete a store by id");
        
        app.MapPut("/stores/{storeId}", UpdateStore)
            .RequireAuthorization()
            .WithTags(Tag)
            .Produces<ApiStore>(200)
            .ProducesValidationProblem(400)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Update a store by id");
        
        app.MapPost("/stores", CreateStore)
            .RequireAuthorization()
            .WithTags(Tag)
            .Accepts<ApiStore>("application/json")
            .Produces<ApiStore>(201)
            .ProducesValidationProblem(400)
            .Produces(401)
            .WithDisplayName("Create a store");

        return app;
    }

    private static async Task<IResult> GetAllStores(ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var stores = await db.Stores
            .Where(s => s.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
        
        return Results.Ok(stores.Select(store => 
            new ApiStore(store.Id, store.Name, store.Description)));
    }

    private static async Task<IResult> GetStore([FromRoute]int storeId, ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var store = await db.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

        if (store == null) return Results.NotFound();
        
        return Results.Ok(
            new ApiStore(store.Id, store.Name, store.Description));
    }

    private static async Task<IResult> DeleteStore([FromRoute]int storeId, ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var store = await db.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

        if (store == null) return Results.NotFound();

        db.Stores.Remove(store);
        await db.SaveChangesAsync();
        
        return Results.Ok();
    }

    private static async Task<IResult> UpdateStore([FromRoute]int storeId, [FromBody]ApiStore apiStore, ClaimsPrincipal principal, GroceryListDb db, HttpContext http, LinkGenerator link)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var store = await db.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

        if (store == null) return Results.NotFound();
        
        if (!MiniValidator.TryValidate(apiStore, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        store.Name = apiStore.Name;
        store.Description = apiStore.Description;
        await db.SaveChangesAsync();

        return Results.Accepted(
            link.GetUriByName(http, ById, new { storeId = store.Id })!, 
            new ApiStore(store.Id, store.Name, store.Description));
    }

    private static async Task<IResult> CreateStore([FromBody]ApiStore apiStore, ClaimsPrincipal principal, GroceryListDb db, HttpContext http, LinkGenerator link)
    {
        if (!MiniValidator.TryValidate(apiStore, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }
        
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));
        var store = new GroceryStore
        {
            UserId = userId,
            Name = apiStore.Name,
            Description = apiStore.Description
        };
        db.Stores.Add(store);
        await db.SaveChangesAsync();

        return Results.Created(
            link.GetUriByName(http, ById, new { storeId = store.Id })!, 
            new ApiStore(store.Id, store.Name, store.Description));
    }
}