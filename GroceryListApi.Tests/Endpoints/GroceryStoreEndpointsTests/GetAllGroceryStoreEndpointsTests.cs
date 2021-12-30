using System.Net;
using System.Threading.Tasks;
using GroceryListApi.Tests.Infrastructure;
using VerifyXunit;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryStoreEndpointsTests;

[UsesVerify]
public class GetAllGroceryStoreEndpointsTests : GroceryStoreEndpointsTests
{
    public GetAllGroceryStoreEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Returns200()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.GetAsync("/stores");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        await Verifier.VerifyJson(await result.Content.ReadAsStringAsync());
    }
}