using System.ComponentModel.DataAnnotations;

namespace GroceryListApi.Endpoints.Schemas;

public record ApiToken(
    [property:Required]string Token);