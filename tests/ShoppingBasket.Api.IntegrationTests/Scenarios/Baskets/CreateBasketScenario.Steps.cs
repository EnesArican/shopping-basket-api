using FluentAssertions;
using ShoppingBasket.Api.Dtos;
using System.Net;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Baskets;

public partial class CreateBasketScenario
{
    private BasketDto? _basketResponse;

    private async Task When_I_send_a_POST_request_to_baskets()
    {
        Client.Should().NotBeNull("Client should be initialized");
        
        Response = await Client!.PostAsync("/baskets", null);
        
        Response.Should().NotBeNull();
    }

    private async Task Then_I_should_receive_a_201_Created_response()
    {
        AssertSuccessResponse(HttpStatusCode.Created);
    }

    private async Task And_the_response_should_contain_a_valid_basket()
    {
        _basketResponse = await DeserializeResponse<BasketDto>();
        
        _basketResponse.Should().NotBeNull();
        _basketResponse!.Id.Should().NotBeEmpty();
        _basketResponse.Items.Should().NotBeNull().And.BeEmpty();
        _basketResponse.DiscountCode.Should().BeNull();
    }
}
