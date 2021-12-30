using System.ComponentModel.DataAnnotations;

namespace GroceryListApi.Endpoints.Schemas;

public record ApiStore(
    int? Id,
    [property:Required] string Name,
    string? Description);