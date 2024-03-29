﻿using System.Net;
using System.Threading.Tasks;
using GroceryListApi.Tests.Infrastructure;
using VerifyXunit;
using Xunit;

namespace GroceryListApi.Tests.Endpoints.GroceryStoreEndpointsTests;

[UsesVerify]
[TestCaseOrderer("GroceryListApi.Tests.Infrastructure.AlphabeticalTestCaseOrderer", "GroceryListApi.Tests")]
public class GetGroceryStoreEndpointsTests : GroceryStoreEndpointsTestsBase
{
    public GetGroceryStoreEndpointsTests(GroceryListApiApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Returns200()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.GetAsync("/stores/1");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        await Verifier.VerifyJson(await result.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task Returns404()
    {
        // Arrange
        await EnsureAuthorizedAsync();
        
        // Act
        var result = await Application.GetAsync("/stores/123");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}