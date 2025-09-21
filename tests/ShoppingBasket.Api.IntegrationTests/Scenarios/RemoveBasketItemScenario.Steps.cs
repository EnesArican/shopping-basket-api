using FluentAssertions;
using ShoppingBasket.Api.Dtos;
using ShoppingBasket.Api.IntegrationTests.Common;
using System.Net;
using System.Net.Http.Json;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios;

public partial class RemoveBasketItemScenario : IntegrationTestBase
{
    private Guid _basketId;
    private Guid _firstItemId = new("11111111-1111-1111-1111-111111111111"); // Hardcoded item from repository
    private Guid _secondItemId = new("22222222-2222-2222-2222-222222222222"); // Hardcoded item from repository
    private BasketDto? _basketResponse;

    private async Task And_a_basket_exists_with_items()
    {
        Client.Should().NotBeNull("Client should be initialized");

        // Create a basket first
        var createBasketResponse = await Client!.PostAsync("/baskets", null);
        createBasketResponse.EnsureSuccessStatusCode();
        
        var basketDto = await createBasketResponse.Content.ReadFromJsonAsync<BasketDto>();
        _basketId = basketDto!.Id;

        // Add items to the basket
        var requestDto = new AddBasketItemsRequestDto(new List<AddBasketItemDto>
        {
            new AddBasketItemDto(_firstItemId, 2, false, null),
            new AddBasketItemDto(_secondItemId, 1, false, null)
        });

        var jsonContent = CreateJsonContent(requestDto);
        var addItemsResponse = await Client.PostAsync($"/baskets/{_basketId}/items", jsonContent);
        addItemsResponse.EnsureSuccessStatusCode();
    }

    private async Task When_I_send_a_DELETE_request_to_remove_item_from_basket()
    {
        Client.Should().NotBeNull("Client should be initialized");
        
        Response = await Client!.DeleteAsync($"/baskets/{_basketId}/items/{_firstItemId}");
        
        Response.Should().NotBeNull();
    }

    private async Task When_I_send_a_DELETE_request_to_remove_item_from_nonexistent_basket()
    {
        Client.Should().NotBeNull("Client should be initialized");
        
        var nonExistentBasketId = Guid.NewGuid();
        Response = await Client!.DeleteAsync($"/baskets/{nonExistentBasketId}/items/{_firstItemId}");
        
        Response.Should().NotBeNull();
    }

    private async Task Then_I_should_receive_a_200_OK_response()
    {
        AssertSuccessResponse(HttpStatusCode.OK);
    }

    private async Task Then_I_should_receive_a_404_NotFound_response()
    {
        AssertErrorResponse(HttpStatusCode.NotFound);
    }

    private async Task And_the_response_should_contain_the_updated_basket_with_decremented_quantity()
    {
        _basketResponse = await DeserializeResponse<BasketDto>();
        
        _basketResponse.Should().NotBeNull();
        _basketResponse!.Id.Should().Be(_basketId);
        _basketResponse.Items.Should().HaveCount(2); // Still 2 items: laptop (qty 1) + mouse (qty 1)
        
        // Verify laptop quantity was decremented from 2 to 1
        var laptopItem = _basketResponse.Items.FirstOrDefault(i => i.Item.Id == _firstItemId);
        laptopItem.Should().NotBeNull();
        laptopItem!.Quantity.Should().Be(1);
        
        // Verify mouse is still there with quantity 1
        var mouseItem = _basketResponse.Items.FirstOrDefault(i => i.Item.Id == _secondItemId);
        mouseItem.Should().NotBeNull();
        mouseItem!.Quantity.Should().Be(1);
    }

    private async Task And_the_response_should_contain_an_error_message()
    {
        Response.Should().NotBeNull();
        Response!.Content.Should().NotBeNull();
        
        var content = await Response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
}
