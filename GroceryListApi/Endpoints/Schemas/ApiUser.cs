using System.ComponentModel.DataAnnotations;

namespace GroceryListApi.Endpoints.Schemas;

public record ApiUser(
    [property:Required]string Username,
    [property:Required]string Password);