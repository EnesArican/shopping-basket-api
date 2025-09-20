using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ShoppingBasket.Api.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios;

public partial class CreateBasketScenario : IAsyncDisposable
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;
    private HttpResponseMessage? _response;
    private BasketDto? _basketResponse;

    private async Task Given_the_API_is_running()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        
        // Verify the client is ready
        _client.Should().NotBeNull();
    }

    private async Task When_I_send_a_POST_request_to_baskets()
    {
        _client.Should().NotBeNull("Client should be initialized");
        
        _response = await _client!.PostAsync("/baskets", null);
        
        _response.Should().NotBeNull();
    }

    private async Task Then_I_should_receive_a_201_Created_response()
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private async Task And_the_response_should_contain_a_valid_basket()
    {
        _response.Should().NotBeNull();
        _response!.Content.Should().NotBeNull();
        
        _response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        
        _basketResponse = await _response.Content.ReadFromJsonAsync<BasketDto>();
        
        _basketResponse.Should().NotBeNull();
        _basketResponse!.Id.Should().NotBeEmpty();
        _basketResponse.Items.Should().NotBeNull().And.BeEmpty();
        _basketResponse.DiscountCode.Should().BeNull();
    }

    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}
