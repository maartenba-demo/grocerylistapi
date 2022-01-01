using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GroceryListApi.Endpoints.Schemas;
using GroceryListApi.Tests.Infrastructure;
using VerifyXunit;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryListItemTests;

[UsesVerify]
[TestCaseOrderer("GroceryListApi.Tests.Infrastructure.AlphabeticalTestCaseOrderer", "GroceryListApi.Tests")]
public class UpdateGroceryListItemEndpointsTests : GroceryListItemEndpointsTestsBase
{
    public UpdateGroceryListItemEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Returns202()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        var createdItemResult = await Application.PostAsJsonAsync("/stores/1/items", 
            new ApiItem(null, null, "Test item", false));
        var createdItem = await createdItemResult.Content.ReadFromJsonAsync<ApiItem>();

        // Act
        var result = await Application.PutAsJsonAsync($"/stores/{createdItem!.StoreId}/items/{createdItem.Id}",
            new ApiItem(null, null, "Updated title", true));
        
        // Assert
        Assert.Equal(HttpStatusCode.Accepted, result.StatusCode);
        await Verifier.VerifyJson(await result.Content.ReadAsStringAsync());
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Returns400ForInvalidModel(string? title)
    {
        // Arrange
        await EnsureAuthorizedAsync();

        // Act
        var result = await Application.PutAsJsonAsync($"/stores/1/items/1",
            new ApiItem(null, null, title!, true));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
    
    [Fact]
    public async Task Returns404()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.PutAsJsonAsync("/stores/1/items/123",
            new ApiItem(null, null, "Test", false));
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}