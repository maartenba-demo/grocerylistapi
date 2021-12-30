using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GroceryListApi.Endpoints.Schemas;
using GroceryListApi.Tests.Infrastructure;
using VerifyXunit;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryStoreEndpointsTests;

[UsesVerify]
[TestCaseOrderer("GroceryListApi.Tests.Infrastructure.AlphabeticalTestCaseOrderer", "GroceryListApi.Tests")]
public class UpdateGroceryStoreEndpointsTests : GroceryStoreEndpointsTestsBase
{
    public UpdateGroceryStoreEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Returns202()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        var createdStoreResult = await Application.PostAsJsonAsync("/stores", 
            new ApiStore(null, "Test store", "This is a test store"));
        var createdStore = createdStoreResult.Content.ReadFromJsonAsync<ApiStore>();
        
        // Act
        var result = await Application.PutAsJsonAsync($"/stores/{createdStore.Id}", 
            new ApiStore(null, "Updated title", "Updated description"));
        
        // Assert
        Assert.Equal(HttpStatusCode.Accepted, result.StatusCode);
        await Verifier.VerifyJson(await result.Content.ReadAsStringAsync());
    }
    
    [Theory]
    [InlineData("", "")]
    [InlineData(null, null)]
    public async Task Returns400ForInvalidModel(string? title, string? description)
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.PutAsJsonAsync($"/stores/1", 
            new ApiStore(null, title!, description));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
    
    [Fact]
    public async Task Returns404()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.PutAsJsonAsync("/stores/123",
            new ApiStore(null, "New test store", "New test description"));
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}