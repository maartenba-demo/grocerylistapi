using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GroceryListApi.Endpoints.Schemas;
using GroceryListApi.Tests.Infrastructure;
using Xunit;

namespace GroceryListApi.Tests.Endpoints;

[TestCaseOrderer("GroceryListApi.Tests.Infrastructure.AlphabeticalTestCaseOrderer", "GroceryListApi.Tests")]
public class TokenEndpointsTests : IClassFixture<GroceryListApiApplicationFactory>
{
    private HttpClient Application { get; }

    public TokenEndpointsTests(GroceryListApiApplicationFactory factory)
    {
        Application = factory.CreateClient();
    }
    
    [Fact]
    public async Task Returns200AndTokenForValidUser()
    {
        // Act
        var result = await Application.PostAsJsonAsync("/token", 
            new ApiUser("test", "1234"));
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var apiToken = await result.Content.ReadFromJsonAsync<ApiToken>();
        Assert.NotNull(apiToken?.Token);
    }
    
    [Theory]
    [InlineData("", "")]
    [InlineData("foo", "")]
    [InlineData(null, null)]
    public async Task Returns401ForInvalidModel(string? username, string? password)
    {
        // Act
        var result = await Application.PostAsJsonAsync("/token", 
            new ApiUser(username!, password!));
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}