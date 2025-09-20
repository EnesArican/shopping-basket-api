using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace ShoppingBasket.Api.IntegrationTests.Scenarios.Items;

[FeatureDescription("Get Items Endpoint Integration Tests")]
public partial class GetItemsScenario : FeatureFixture
{
    [Scenario]
    public async Task Getting_all_available_items_successfully()
    {
        await Runner.RunScenarioAsync(
            _ => Given_the_API_is_running(),
            _ => When_I_send_a_GET_request_to_items(),
            _ => Then_I_should_receive_a_200_OK_response(),
            _ => And_the_response_should_contain_a_list_of_items()
        );
    }
}
