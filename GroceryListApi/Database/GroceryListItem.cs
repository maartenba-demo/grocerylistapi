using System.ComponentModel.DataAnnotations;

namespace GroceryListApi.Database;

public class GroceryListItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    public int StoreId { get; set; }
    public GroceryStore Store { get; set; } = null!;
    
    [Required] public string Title { get; set; } = null!;
    public bool IsComplete { get; set; }
}