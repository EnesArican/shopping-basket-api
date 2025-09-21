using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShoppingBasket.Api.Dtos;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Baskets;

public partial class ApplyDiscountScenario
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

    private async Task When_I_apply_a_valid_discount_code()
    {
        var request = new ApplyDiscountRequestDto("SAVE20");
        var content = CreateJsonContent(request);
        Response = await Client!.PostAsync($"/baskets/{_basketId}/discount", content);
    }

    private async Task When_I_apply_an_invalid_discount_code()
    {
        var request = new ApplyDiscountRequestDto("INVALID123");
        var content = CreateJsonContent(request);
        Response = await Client!.PostAsync($"/baskets/{_basketId}/discount", content);
    }

    private async Task Then_the_discount_should_be_applied_successfully()
    {
        Response.Should().NotBeNull();
        Response!.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedBasket = await DeserializeResponse<BasketDto>();
        updatedBasket.Should().NotBeNull();
        updatedBasket!.Id.Should().Be(_basketId);
        updatedBasket.DiscountCode.Should().Be("SAVE20");
    }

    private async Task And_the_basket_should_contain_the_discount_code()
    {
        var basketWithDiscount = await DeserializeResponse<BasketDto>();
        basketWithDiscount!.DiscountCode.Should().Be("SAVE20");
    }

    private async Task Then_the_request_should_fail_with_invalid_discount_code_error()
    {
        Response.Should().NotBeNull();
        Response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var errorResponse = await DeserializeResponse<ErrorResponseDto>();
        
        errorResponse.Should().NotBeNull();
        errorResponse!.ErrorMessage.Should().Be("invalid_discount_code");
    }
}
