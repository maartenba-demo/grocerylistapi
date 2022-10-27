using System.Security.Claims;
using GroceryListApi.Endpoints.Schemas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

namespace GroceryListApi.Endpoints;

public static class GroceryListItemEndpoints
{
    private const string Tag = "Items";
    private const string ById = "stores/byid/items/byid";

    public static WebApplication MapGroceryListItemEndpoints(this WebApplication app)
    {
        var routes = app.MapGroup("/stores/{storeId:int}")
            .RequireAuthorization()
            .WithTags(Tag);

        routes.MapGet("/items", GetAllItems)
            .Produces<ICollection<ApiItem>>(200)
            .Produces(401)
            .WithDisplayName("Get all items for a store");
        
        routes.MapGet("/items/{itemId}", GetItem)
            .Produces<ApiItem>(200)
            .Produces(404)
            .Produces(401)
            .WithName(ById)
            .WithDisplayName("Get an item by id");
        
        routes.MapDelete("/items/{itemId}", DeleteItem)
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Delete an item by id");
        
        routes.MapPut("/items/{itemId}", UpdateItem)
            .Produces<ApiItem>(200)
            .ProducesValidationProblem(400)
            .Produces(404)
            .Produces(401)
            .WithDisplayName("Update an item by id");
        
        routes.MapPost("/items", CreateItem)
            .Accepts<ApiItem>("application/json")
            .Produces<ApiItem>(201)
            .ProducesValidationProblem(400)
            .Produces(401)
            .WithDisplayName("Create an item");

        return app;
    }

    private static async Task<IResult> GetAllItems([FromRoute]int storeId, ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var items = await db.Items
            .Where(i => i.UserId == userId && i.StoreId == storeId)
            .AsNoTracking()
            .ToListAsync();
        
        return Results.Ok(items.Select(item => 
            new ApiItem(item.Id, item.StoreId, item.Title, item.IsComplete)));
    }

    private static async Task<IResult> GetItem([FromRoute]int storeId, [FromRoute]int itemId, ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var item = await db.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == itemId && i.UserId == userId && i.StoreId == storeId);

        if (item == null) return Results.NotFound();
        
        return Results.Ok(
            new ApiItem(item.Id, item.StoreId, item.Title, item.IsComplete));
    }

    private static async Task<IResult> DeleteItem([FromRoute]int storeId, [FromRoute]int itemId, ClaimsPrincipal principal, GroceryListDb db)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var item = await db.Items
            .FirstOrDefaultAsync(i => i.Id == itemId && i.UserId == userId && i.StoreId == storeId);

        if (item == null) return Results.NotFound();

        db.Items.Remove(item);
        await db.SaveChangesAsync();
        
        return Results.Ok();
    }

    private static async Task<IResult> UpdateItem([FromRoute]int storeId, [FromRoute]int itemId, [FromBody]ApiItem apiItem, ClaimsPrincipal principal, GroceryListDb db, HttpContext http, LinkGenerator link)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));

        var item = await db.Items
            .FirstOrDefaultAsync(i => i.Id == itemId && i.UserId == userId && i.StoreId == storeId);

        if (item == null) return Results.NotFound();
        
        if (!MiniValidator.TryValidate(apiItem, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }

        item.Title = apiItem.Title;
        item.IsComplete = apiItem.IsComplete;

        await db.SaveChangesAsync();

        return Results.Accepted(
            link.GetUriByName(http, ById, new { storeId = item.StoreId, itemId = item.Id })!, 
            new ApiItem(item.Id, item.StoreId, item.Title, item.IsComplete));
    }

    private static async Task<IResult> CreateItem([FromRoute]int storeId, [FromBody]ApiItem apiItem, ClaimsPrincipal principal, GroceryListDb db, HttpContext http, LinkGenerator link)
    {
        var userId = int.Parse(principal.GetClaimValue(GroceryClaimTypes.UserId));
        
        var store = await db.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

        if (store == null) return Results.NotFound();
        
        if (!MiniValidator.TryValidate(apiItem, out var validationErrors))
        {
            return Results.ValidationProblem(validationErrors);
        }
        
        var item = new GroceryListItem
        {
            UserId = userId,
            StoreId = storeId,
            Title = apiItem.Title,
            IsComplete = apiItem.IsComplete
        };
        db.Items.Add(item);
        await db.SaveChangesAsync();

        return Results.Created(
            link.GetUriByName(http, ById, new { storeId = item.StoreId, itemId = item.Id })!, 
            new ApiItem(item.Id, item.StoreId, item.Title, item.IsComplete));
    }
}