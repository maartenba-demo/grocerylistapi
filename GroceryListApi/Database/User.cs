using System.ComponentModel.DataAnnotations;

namespace GroceryListApi.Database;

public class User
{
    public int Id { get; set; }
    [Required] public string Username { get; set; } = null!;
    [Required] public string Name { get; set; } = null!;
    [Required] public string Email { get; set; } = null!;
    
    public ICollection<GroceryStore> Stores { get; set; } = null!;
    public ICollection<GroceryListItem> Items { get; set; } = null!;
}