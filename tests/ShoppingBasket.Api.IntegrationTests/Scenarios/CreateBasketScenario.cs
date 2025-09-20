using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios;

[FeatureDescription("Create Basket Endpoint Integration Tests")]
public partial class CreateBasketScenario : FeatureFixture
{
    [Scenario]
    public async Task Creating_a_new_basket_successfully()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => When_I_send_a_POST_request_to_baskets(),
            _ => Then_I_should_receive_a_201_Created_response(),
            _ => And_the_response_should_contain_a_valid_basket()
        );
    }
}
