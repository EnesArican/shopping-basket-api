using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShoppingBasket.Api.Dtos;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Baskets;

public partial class AddBasketItemsScenario
{
    private Guid _basketId;
    private Guid _firstItemId = new("11111111-1111-1111-1111-111111111111"); // Hardcoded item from repository
    private Guid _secondItemId = new("22222222-2222-2222-2222-222222222222"); // Hardcoded item from repository

    private async Task And_a_basket_exists()
    {
        // Create a basket first
        var createBasketResponse = await Client!.PostAsync("/baskets", null);
        createBasketResponse.EnsureSuccessStatusCode();
        
        var basketDto = await createBasketResponse.Content.ReadFromJsonAsync<BasketDto>();
        _basketId = basketDto!.Id;
    }

    private async Task When_I_send_a_POST_request_to_add_items_to_basket()
    {
        var requestDto = new AddBasketItemsRequestDto(new List<AddBasketItemDto>
        {
            new AddBasketItemDto(_firstItemId, 2, false, null),
            new AddBasketItemDto(_secondItemId, 1, true, 25)
        });

        var content = CreateJsonContent(requestDto);
        Response = await Client!.PostAsync($"/baskets/{_basketId}/items", content);
    }

    private async Task When_I_send_a_POST_request_to_add_items_to_nonexistent_basket()
    {
        var nonExistentBasketId = Guid.NewGuid();
        var requestDto = new AddBasketItemsRequestDto(new List<AddBasketItemDto>
        {
            new AddBasketItemDto(_firstItemId, 1, false, null)
        });

        var content = CreateJsonContent(requestDto);
        Response = await Client!.PostAsync($"/baskets/{nonExistentBasketId}/items", content);
    }

    private async Task Then_I_should_receive_a_200_OK_response()
    {
        AssertSuccessResponse(HttpStatusCode.OK);
    }

    private async Task Then_I_should_receive_a_404_NotFound_response()
    {
        AssertErrorResponse(HttpStatusCode.NotFound);
    }

    private async Task And_the_response_should_contain_the_updated_basket_with_items()
    {
        var basketDto = await DeserializeResponse<BasketDto>();

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
        Response.Should().NotBeNull();
        Response!.Content.Should().NotBeNull();
        
        var content = await Response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
}
