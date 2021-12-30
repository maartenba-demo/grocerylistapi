using System.ComponentModel.DataAnnotations;

namespace GroceryListApi.Database;

public class GroceryStore
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required] public string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    public ICollection<GroceryListItem> Items { get; set; } = null!;
}