using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ShoppingBasket.Api.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Items;

public partial class GetItemsScenario : IAsyncDisposable
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;
    private HttpResponseMessage? _response;
    private HttpResponseMessage? _secondResponse;
    private List<ItemDto>? _itemsResponse;
    private List<ItemDto>? _secondItemsResponse;

    private async Task Given_the_API_is_running()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        
        // Verify the client is ready
        _client.Should().NotBeNull();
    }

    private async Task When_I_send_a_GET_request_to_items()
    {
        _client.Should().NotBeNull("Client should be initialized");
        
        _response = await _client!.GetAsync("/items");
        
        _response.Should().NotBeNull();
    }

    private async Task And_I_send_another_GET_request_to_items()
    {
        _client.Should().NotBeNull("Client should be initialized");
        
        _secondResponse = await _client!.GetAsync("/items");
        
        _secondResponse.Should().NotBeNull();
    }

    private async Task Then_I_should_receive_a_200_OK_response()
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task And_the_response_should_contain_a_list_of_items()
    {
        _response.Should().NotBeNull();
        _response!.Content.Should().NotBeNull();
        
        _response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        
        _itemsResponse = await _response.Content.ReadFromJsonAsync<List<ItemDto>>();
        
        _itemsResponse.Should().NotBeNull();
        _itemsResponse!.Should().NotBeEmpty();
        _itemsResponse.Should().HaveCountGreaterThan(0);
        
        // Verify each item has required properties
        foreach (var item in _itemsResponse)
        {
            item.Id.Should().NotBeEmpty();
            item.Name.Should().NotBeNullOrWhiteSpace();
            item.Price.Should().BeGreaterThan(0);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        _response?.Dispose();
        _secondResponse?.Dispose();
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}
