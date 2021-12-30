using System.ComponentModel.DataAnnotations;

namespace GroceryListApi.Endpoints.Schemas;

public record ApiItem(
    int? Id,
    int? StoreId,
    [property:Required] string Title,
    bool IsComplete);