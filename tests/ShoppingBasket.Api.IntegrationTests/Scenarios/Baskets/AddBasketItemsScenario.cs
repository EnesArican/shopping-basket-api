using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using ShoppingBasket.Api.IntegrationTests.Common;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Baskets;

[FeatureDescription("Add Basket Items Endpoint Integration Tests")]
public partial class AddBasketItemsScenario : IntegrationTestBase
{
    [Scenario]
    public async Task Adding_items_to_basket_successfully()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => And_a_basket_exists(),
            _ => When_I_send_a_POST_request_to_add_items_to_basket(),
            _ => Then_I_should_receive_a_200_OK_response(),
            _ => And_the_response_should_contain_the_updated_basket_with_items()
        );
    }

    [Scenario]
    public async Task Adding_items_to_nonexistent_basket_returns_error()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => When_I_send_a_POST_request_to_add_items_to_nonexistent_basket(),
            _ => Then_I_should_receive_a_404_NotFound_response(),
            _ => And_the_response_should_contain_an_error_message()
        );
    }
}
