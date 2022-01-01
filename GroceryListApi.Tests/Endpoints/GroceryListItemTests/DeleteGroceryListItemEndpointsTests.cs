using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GroceryListApi.Endpoints.Schemas;
using GroceryListApi.Tests.Infrastructure;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryListItemTests;

[TestCaseOrderer("GroceryListApi.Tests.Infrastructure.AlphabeticalTestCaseOrderer", "GroceryListApi.Tests")]
public class DeleteGroceryListItemEndpointsTests : GroceryListItemEndpointsTestsBase
{
    public DeleteGroceryListItemEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Returns200()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        var createdItemResult = await Application.PostAsJsonAsync("/stores/1/items", 
            new ApiItem(null, null, "Test item", true));
        var createdItem = await createdItemResult.Content.ReadFromJsonAsync<ApiItem>();
        
        // Act
        var result = await Application.DeleteAsync($"/stores/1/items/{createdItem!.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
    
    [Fact]
    public async Task Returns404()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.DeleteAsync("/stores/1/items/123");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}