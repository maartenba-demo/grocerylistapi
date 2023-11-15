using System.Security.Claims;
using GroceryListApi.Endpoints.Schemas;
using Microsoft.AspNetCore.Http.HttpResults;
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
        var routes = app.MapGroup("/stores")
            .RequireAuthorization()
            .WithTags(Tag);

        routes.MapGet("/", GetAllStores)
            .WithDisplayName("Get all stores");
        
        routes.MapGet("/{storeId:int}", GetStore)
            .WithName(ById)
            .WithDisplayName("Get a store by id");
        
        routes.MapDelete("/{storeId:int}", DeleteStore)
            .WithDisplayName("Delete a store by id");
        
        routes.MapPut("/{storeId:int}", UpdateStore)
            .Accepts<ApiStore>("application/json")
            .WithDisplayName("Update a store by id");
        
        routes.MapPost("/", CreateStore)
            .Accepts<ApiStore>("application/json")
            .WithDisplayName("Create a store");

        return app;
    }

    private static async Task<Results<Ok<IEnumerable<ApiStore>>, Unauthorized>> GetAllStores(ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        // var stores = await db.Stores
        //     .FromSqlInterpolated($"SELECT * FROM Stores WHERE UserId = '{userId}'")
        //     .ToListAsync();

        var stores = await db.Stores
            .Where(s => s.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
        
        return TypedResults.Ok(stores.Select(store =>
            new ApiStore(store.Id, store.Name, store.Description)));
    }

    private static async Task<Results<Ok<ApiStore>, NotFound, Unauthorized>> GetStore([FromRoute]int storeId, ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var store = await db.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

        if (store == null) return TypedResults.NotFound();
        
        return TypedResults.Ok(
            new ApiStore(store.Id, store.Name, store.Description));
    }

    private static async Task<Results<Ok, NotFound, Unauthorized>> DeleteStore([FromRoute]int storeId, ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var store = await db.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

        if (store == null) return TypedResults.NotFound();

        db.Stores.Remove(store);
        await db.SaveChangesAsync();
        
        return TypedResults.Ok();
    }

    private static async Task<Results<Accepted<ApiStore>, ValidationProblem, NotFound, Unauthorized>> UpdateStore([FromRoute]int storeId, [FromBody]ApiStore apiStore, ClaimsPrincipal principal, GroceryListDb db, HttpContext http, LinkGenerator link)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var store = await db.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

        if (store == null) return TypedResults.NotFound();
        
        if (!MiniValidator.TryValidate(apiStore, out var validationErrors))
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        store.Name = apiStore.Name;
        store.Description = apiStore.Description;
        await db.SaveChangesAsync();

        return TypedResults.Accepted(
            link.GetUriByName(http, ById, new { storeId = store.Id })!, 
            new ApiStore(store.Id, store.Name, store.Description));
    }

    private static async Task<Results<Created<ApiStore>, ValidationProblem, NotFound, Unauthorized>> CreateStore([FromBody]ApiStore apiStore, ClaimsPrincipal principal, GroceryListDb db, HttpContext http, LinkGenerator link)
    {
        if (!MiniValidator.TryValidate(apiStore, out var validationErrors))
        {
            return TypedResults.ValidationProblem(validationErrors);
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

        return TypedResults.Created(
            link.GetUriByName(http, ById, new { storeId = store.Id })!, 
            new ApiStore(store.Id, store.Name, store.Description));
    }
}