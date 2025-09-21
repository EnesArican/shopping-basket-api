using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using ShoppingBasket.Api.IntegrationTests.Common;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios;

[FeatureDescription("Set Shipping Endpoint Integration Tests")]
public partial class SetShippingScenario : IntegrationTestBase
{
    [Scenario]
    public async Task SetShippingForBasket_WhenValidRequest_ShouldSetShippingSuccessfully()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => Given_a_basket_exists(),
            _ => When_I_set_shipping_to_UK(),
            _ => Then_the_shipping_should_be_set_successfully(),
            _ => And_the_basket_should_contain_UK_shipping_details());
    }

    [Scenario]
    public async Task SetShippingForBasket_WhenInvalidRequest_ShouldReturnError()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => Given_a_basket_exists(),
            _ => When_I_set_shipping_with_invalid_country(),
            _ => Then_the_request_should_fail_with_invalid_shipping_country_error());
    }
}
