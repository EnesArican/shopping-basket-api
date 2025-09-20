using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using ShoppingBasket.Api.IntegrationTests.Common;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios;

[FeatureDescription("Remove Basket Item Endpoint Integration Tests")]
public partial class RemoveBasketItemScenario : IntegrationTestBase
{
    [Scenario]
    public async Task Removing_item_from_basket_decrements_quantity()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => And_a_basket_exists_with_items(),
            _ => When_I_send_a_DELETE_request_to_remove_item_from_basket(),
            _ => Then_I_should_receive_a_200_OK_response(),
            _ => And_the_response_should_contain_the_updated_basket_with_decremented_quantity()
        );
    }

    [Scenario]
    public async Task Removing_item_from_nonexistent_basket_returns_error()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => When_I_send_a_DELETE_request_to_remove_item_from_nonexistent_basket(),
            _ => Then_I_should_receive_a_404_NotFound_response(),
            _ => And_the_response_should_contain_an_error_message()
        );
    }
}
