using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShoppingBasket.Api.Dtos;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios;

public partial class SetShippingScenario
{
    private Guid _basketId;

    private async Task Given_a_basket_exists()
    {
        // Create a basket first
        var createResponse = await Client!.PostAsync("/baskets", null);
        createResponse.EnsureSuccessStatusCode();
        
        var basketDto = await createResponse.Content.ReadFromJsonAsync<BasketDto>();
        _basketId = basketDto!.Id;
    }

    private async Task When_I_set_shipping_to_UK()
    {
        var request = new SetShippingRequestDto("UK");
        var content = CreateJsonContent(request);
        Response = await Client!.PostAsync($"/baskets/{_basketId}/shipping", content);
    }

    private async Task When_I_set_shipping_with_invalid_country()
    {
        var request = new SetShippingRequestDto("");
        var content = CreateJsonContent(request);
        Response = await Client!.PostAsync($"/baskets/{_basketId}/shipping", content);
    }

    private async Task Then_the_shipping_should_be_set_successfully()
    {
        Response.Should().NotBeNull();
        Response!.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedBasket = await DeserializeResponse<BasketDto>();
        updatedBasket.Should().NotBeNull();
        updatedBasket!.Id.Should().Be(_basketId);
        updatedBasket.ShippingCountry.Should().Be("UK");
        updatedBasket.ShippingCost.Should().Be(5.99m);
    }

    private async Task And_the_basket_should_contain_UK_shipping_details()
    {
        var basketWithShipping = await DeserializeResponse<BasketDto>();
        basketWithShipping!.ShippingCountry.Should().Be("UK");
        basketWithShipping.ShippingCost.Should().Be(5.99m);
    }

    private async Task Then_the_request_should_fail_with_invalid_shipping_country_error()
    {
        Response.Should().NotBeNull();
        Response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var errorResponse = await DeserializeResponse<ErrorResponseDto>();
        
        errorResponse.Should().NotBeNull();
        errorResponse!.ErrorMessage.Should().Be("invalid_shipping_country");
    }
}
