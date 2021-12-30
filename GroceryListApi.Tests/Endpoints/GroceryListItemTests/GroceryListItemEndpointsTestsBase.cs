using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GroceryListApi.Endpoints.Schemas;
using GroceryListApi.Tests.Infrastructure;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryListItemTests;

[TestCaseOrderer("GroceryListApi.Tests.Infrastructure.AlphabeticalTestCaseOrderer", "GroceryListApi.Tests")]
public abstract class GroceryListItemEndpointsTestsBase : IClassFixture<GroceryListApiApplicationFactory>
{
    protected HttpClient Application { get; }

    public GroceryListItemEndpointsTestsBase(GroceryListApiApplicationFactory factory)
    {
        Application = factory.CreateClient();
    }

    public async Task EnsureAuthorizedAsync()
    {
        var tokenResult = await Application.PostAsJsonAsync("/token", 
            new ApiUser("test", "1234"));
        
        var apiToken = await tokenResult.Content.ReadFromJsonAsync<ApiToken>();
        Application.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken!.Token);
    }
}