using FluentAssertions;
using ShoppingBasket.Api.Dtos;
using System.Net;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Items;

public partial class GetItemsScenario
{
    private List<ItemDto>? _itemsResponse;

    private async Task When_I_send_a_GET_request_to_items()
    {
        Client.Should().NotBeNull("Client should be initialized");
        
        Response = await Client!.GetAsync("/items");
        
        Response.Should().NotBeNull();
    }

    private async Task Then_I_should_receive_a_200_OK_response()
    {
        AssertSuccessResponse(HttpStatusCode.OK);
    }

    private async Task And_the_response_should_contain_a_list_of_items()
    {
        _itemsResponse = await DeserializeResponse<List<ItemDto>>();
        
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
}
