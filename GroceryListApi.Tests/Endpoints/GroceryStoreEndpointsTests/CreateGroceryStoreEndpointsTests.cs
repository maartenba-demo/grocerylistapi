using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GroceryListApi.Endpoints.Schemas;
using GroceryListApi.Tests.Infrastructure;
using VerifyXunit;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryStoreEndpointsTests;

[UsesVerify]
public class CreateGroceryStoreEndpointsTests : GroceryStoreEndpointsTests
{
    public CreateGroceryStoreEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Returns201()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.PostAsJsonAsync("/stores", 
            new ApiStore(null, "Test store", "This is a test store"));
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
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
        var result = await Application.PostAsJsonAsync("/stores", 
            new ApiStore(null, title!, description));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}