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
public class CreateGroceryListItemEndpointsTests : GroceryListItemEndpointsTestsBase
{
    public CreateGroceryListItemEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Returns201()
    {
        // Arrange
        await EnsureAuthorizedAsync();

        // Act
        var result = await Application.PostAsJsonAsync("/stores/1/items",
            new ApiItem(null, null, "Test item", false));

        // Assert
        await Verifier.Verify(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Returns400ForInvalidModel(string? title)
    {
        // Arrange
        await EnsureAuthorizedAsync();

        // Act
        var result = await Application.PostAsJsonAsync("/stores/1/items",
            new ApiItem(null, null, title!, true));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}