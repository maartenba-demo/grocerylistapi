using System.Net;
using System.Threading.Tasks;
using GroceryListApi.Tests.Infrastructure;
using VerifyXunit;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryListItemTests;

[UsesVerify]
[TestCaseOrderer("GroceryListApi.Tests.Infrastructure.AlphabeticalTestCaseOrderer", "GroceryListApi.Tests")]
public class GetAllGroceryListItemEndpointsTests : GroceryListItemEndpointsTestsBase
{
    public GetAllGroceryListItemEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Returns200()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.GetAsync("/stores/1/items");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        await Verifier.VerifyJson(await result.Content.ReadAsStringAsync());
    }
}