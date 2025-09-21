using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using ShoppingBasket.Api.IntegrationTests.Common;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Baskets;

[FeatureDescription("Apply Discount Endpoint Integration Tests")]
public partial class ApplyDiscountScenario : IntegrationTestBase
{
    [Scenario]
    public async Task ApplyDiscountToBasket_WhenValidRequest_ShouldApplyDiscountSuccessfully()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => Given_a_basket_exists(),
            _ => When_I_apply_a_valid_discount_code(),
            _ => Then_the_discount_should_be_applied_successfully(),
            _ => And_the_basket_should_contain_the_discount_code());
    }

    [Scenario]
    public async Task ApplyDiscountToBasket_WhenInvalidRequest_ShouldReturnError()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => Given_a_basket_exists(),
            _ => When_I_apply_an_invalid_discount_code(),
            _ => Then_the_request_should_fail_with_invalid_discount_code_error());
    }
}
