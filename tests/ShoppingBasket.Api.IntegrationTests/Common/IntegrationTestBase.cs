using FluentAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ShoppingBasket.Api.IntegrationTests.Common;

public abstract class IntegrationTestBase : FeatureFixture, IAsyncDisposable
{
    protected WebApplicationFactory<Program>? Factory;
    protected HttpClient? Client;
    protected HttpResponseMessage? Response;

    protected JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected virtual async Task Given_the_API_is_running()
    {
        Factory = new WebApplicationFactory<Program>();
        Client = Factory.CreateClient();
        
        Client.Should().NotBeNull();
    }

    protected async Task<T?> DeserializeResponse<T>()
    {
        Response.Should().NotBeNull();
        Response!.Content.Should().NotBeNull();
        
        var content = await Response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    protected StringContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    protected virtual void AssertSuccessResponse(HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
    {
        Response.Should().NotBeNull();
        Response!.StatusCode.Should().Be(expectedStatusCode);
        Response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    protected virtual void AssertErrorResponse(HttpStatusCode expectedStatusCode)
    {
        Response.Should().NotBeNull();
        Response!.StatusCode.Should().Be(expectedStatusCode);
    }

    public virtual async ValueTask DisposeAsync()
    {
        Client?.Dispose();
        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}
