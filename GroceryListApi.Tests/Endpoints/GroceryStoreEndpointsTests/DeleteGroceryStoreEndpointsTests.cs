using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GroceryListApi.Endpoints.Schemas;
using GroceryListApi.Tests.Infrastructure;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryStoreEndpointsTests;

public class DeleteGroceryStoreEndpointsTests : GroceryStoreEndpointsTests
{
    public DeleteGroceryStoreEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Returns200()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        var createdStoreResult = await Application.PostAsJsonAsync("/stores", 
            new ApiStore(null, "Test store", "This is a test store"));
        var createdStore = createdStoreResult.Content.ReadFromJsonAsync<ApiStore>();
        
        // Act
        var result = await Application.DeleteAsync($"/stores/{createdStore.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
    
    [Fact]
    public async Task Returns404()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.DeleteAsync("/stores/123");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}