using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShoppingBasket.Api.Dtos;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios;

public partial class GetBasketTotalScenario
{
    private Guid _basketId;
    private Guid _firstItemId = new("11111111-1111-1111-1111-111111111111"); // Laptop - $1000.00
    private Guid _secondItemId = new("22222222-2222-2222-2222-222222222222"); // Wireless Mouse - $30.00

    private async Task And_a_basket_exists_with_items()
    {
        // Create a basket first
        var createBasketResponse = await Client!.PostAsync("/baskets", null);
        createBasketResponse.EnsureSuccessStatusCode();
        
        var basketDto = await createBasketResponse.Content.ReadFromJsonAsync<BasketDto>();
        _basketId = basketDto!.Id;

        // Add items to the basket (Laptop qty:1, Mouse qty:2 with 10% discount)
        var requestDto = new AddBasketItemsRequestDto(new List<AddBasketItemDto>
        {
            new AddBasketItemDto(_firstItemId, 1, false, null), // Laptop: 1000.00
            new AddBasketItemDto(_secondItemId, 2, true, 10)    // Mouse: 30.00 * 2 = 60.00, with 10% discount = 54.00
        });

        var content = CreateJsonContent(requestDto);
        var addItemsResponse = await Client!.PostAsync($"/baskets/{_basketId}/items", content);
        addItemsResponse.EnsureSuccessStatusCode();
    }

    private async Task When_I_send_a_GET_request_to_get_basket_total()
    {
        Response = await Client!.GetAsync($"/baskets/{_basketId}/total");
    }

    private async Task When_I_send_a_GET_request_to_get_total_for_nonexistent_basket()
    {
        var nonExistentBasketId = Guid.NewGuid();
        Response = await Client!.GetAsync($"/baskets/{nonExistentBasketId}/total");
    }

    private async Task Then_I_should_receive_a_200_OK_response()
    {
        Response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task Then_I_should_receive_a_404_NotFound_response()
    {
        Response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task And_the_response_should_contain_basket_total_with_correct_calculations()
    {
        var basketTotal = await DeserializeResponse<BasketTotalDto>();
        
        basketTotal.Should().NotBeNull();
        basketTotal!.BasketId.Should().Be(_basketId);
        basketTotal.TotalItems.Should().Be(3); // 1 laptop + 2 mice
        
        // Expected calculations:
        // Laptop: 1000.00 (no discount)
        // Mouse: 30.00 * 2 = 60.00, with 10% discount = 54.00 (60.00 - 6.00 = 54.00)
        // SubTotal: 1000.00 + 54.00 = 1054.00
        basketTotal.SubTotal.Should().BeApproximately(1054.00m, 0.01m);
        
        // VAT: 1054.00 * 0.20 = 210.80
        basketTotal.VatAmount.Should().BeApproximately(210.80m, 0.01m);
        
        // Total with VAT: 1054.00 + 210.80 = 1264.80
        basketTotal.TotalWithVat.Should().BeApproximately(1264.80m, 0.01m);
        basketTotal.TotalWithoutVat.Should().BeApproximately(1054.00m, 0.01m);
    }

    private async Task And_the_response_should_contain_an_error_message()
    {
        var errorResponse = await DeserializeResponse<ErrorResponseDto>();
        
        errorResponse.Should().NotBeNull();
        errorResponse!.ErrorMessage.Should().NotBeEmpty();
    }
}
