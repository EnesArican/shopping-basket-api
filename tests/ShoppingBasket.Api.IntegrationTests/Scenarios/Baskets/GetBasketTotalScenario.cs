using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using ShoppingBasket.Api.IntegrationTests.Common;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Baskets;

[FeatureDescription("Get Basket Total Endpoint Integration Tests")]
public partial class GetBasketTotalScenario : IntegrationTestBase
{
    [Scenario]
    public async Task Getting_basket_total_successfully()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => And_a_basket_exists_with_items(),
            _ => When_I_send_a_GET_request_to_get_basket_total(),
            _ => Then_I_should_receive_a_200_OK_response(),
            _ => And_the_response_should_contain_basket_total_with_correct_calculations()
        );
    }

    [Scenario]
    public async Task Getting_total_for_nonexistent_basket_returns_error()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => When_I_send_a_GET_request_to_get_total_for_nonexistent_basket(),
            _ => Then_I_should_receive_a_404_NotFound_response(),
            _ => And_the_response_should_contain_an_error_message()
        );
    }
}
