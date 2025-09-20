using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ShoppingBasket.Api;
using ShoppingBasket.Api.Dtos;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios;

public partial class AddBasketItemsScenario : IAsyncDisposable
{
    private WebApplicationFactory<Program> _factory = default!;
    private HttpClient _client = default!;
    private HttpResponseMessage _response = default!;
    private Guid _basketId;
    private Guid _firstItemId = new("11111111-1111-1111-1111-111111111111"); // Hardcoded item from repository
    private Guid _secondItemId = new("22222222-2222-2222-2222-222222222222"); // Hardcoded item from repository

    private async Task Given_the_API_is_running()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    private async Task And_a_basket_exists()
    {
        // Create a basket first
        var createBasketResponse = await _client.PostAsync("/baskets", null);
        createBasketResponse.EnsureSuccessStatusCode();
        
        var createBasketContent = await createBasketResponse.Content.ReadAsStringAsync();
        var basketDto = JsonSerializer.Deserialize<BasketDto>(createBasketContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        _basketId = basketDto!.Id;
    }

    private async Task When_I_send_a_POST_request_to_add_items_to_basket()
    {
        var requestDto = new AddBasketItemsRequestDto(new List<AddBasketItemDto>
        {
            new AddBasketItemDto(_firstItemId, 2, false, null),
            new AddBasketItemDto(_secondItemId, 1, true, 25)
        });

        var jsonContent = JsonSerializer.Serialize(requestDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _response = await _client.PostAsync($"/baskets/{_basketId}/items", content);
    }

    private async Task When_I_send_a_POST_request_to_add_items_to_nonexistent_basket()
    {
        var nonExistentBasketId = Guid.NewGuid();
        var requestDto = new AddBasketItemsRequestDto(new List<AddBasketItemDto>
        {
            new AddBasketItemDto(_firstItemId, 1, false, null)
        });

        var jsonContent = JsonSerializer.Serialize(requestDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _response = await _client.PostAsync($"/baskets/{nonExistentBasketId}/items", content);
    }

    private async Task Then_I_should_receive_a_200_OK_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task Then_I_should_receive_a_404_NotFound_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task And_the_response_should_contain_the_updated_basket_with_items()
    {
        var responseContent = await _response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();

        var basketDto = JsonSerializer.Deserialize<BasketDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        basketDto.Should().NotBeNull();
        basketDto!.Id.Should().Be(_basketId);
        basketDto.Items.Should().HaveCount(2);
        
        // Verify first item
        var firstItem = basketDto.Items.First(i => i.Item.Id == _firstItemId);
        firstItem.Quantity.Should().Be(2);
        firstItem.IsDiscounted.Should().BeFalse();
        
        // Verify second item
        var secondItem = basketDto.Items.First(i => i.Item.Id == _secondItemId);
        secondItem.Quantity.Should().Be(1);
        secondItem.IsDiscounted.Should().BeTrue();
        secondItem.DiscountPercentage.Should().Be(25);
    }

    private async Task And_the_response_should_contain_an_error_message()
    {
        var responseContent = await _response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();

        var errorDto = JsonSerializer.Deserialize<ErrorResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorDto.Should().NotBeNull();
        errorDto!.ErrorMessage.Should().NotBeNullOrEmpty();
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
