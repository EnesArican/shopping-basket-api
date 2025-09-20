using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingBasket.Application.Domain.Features.Basket.AddBasketItem;
using ShoppingBasket.Application.Domain.Features.Basket.CreateBasket;
using ShoppingBasket.Application.Domain.Features.Item.GetItems;
using ShoppingBasket.Application.Infrastructure.Repositories;
using ShoppingBasket.Application.Infrastructure.Repositories.InMemory;

namespace ShoppingBasket.Application.Components.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<CreateBasketHandler>()
            .AddSingleton<AddBasketItemHandler>()
            .AddSingleton<GetItemsHandler>()
            ;


        services
           .AddSingleton<IBasketsRepository, InMemoryBasketsRepository>()
           .AddSingleton<IItemsRepository, InMemoryItemsRepository>();

        return services;
    }
}
