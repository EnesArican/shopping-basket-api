using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingBasket.Application.Domain.Features.Baskets.AddBasketItems;
using ShoppingBasket.Application.Domain.Features.Baskets.ApplyDiscount;
using ShoppingBasket.Application.Domain.Features.Baskets.CreateBasket;
using ShoppingBasket.Application.Domain.Features.Baskets.GetBasketTotal;
using ShoppingBasket.Application.Domain.Features.Baskets.RemoveBasketItem;
using ShoppingBasket.Application.Domain.Features.Items.GetItems;
using ShoppingBasket.Application.Infrastructure.Repositories;
using ShoppingBasket.Application.Infrastructure.Repositories.InMemory;

namespace ShoppingBasket.Application.Components.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<CreateBasketHandler>()
            .AddSingleton<AddBasketItemsHandler>()
            .AddSingleton<RemoveBasketItemHandler>()
            .AddSingleton<GetBasketTotalHandler>()
            .AddSingleton<ApplyDiscountHandler>()
            .AddSingleton<GetItemsHandler>()
            ;


        services
           .AddSingleton<IBasketsRepository, InMemoryBasketsRepository>()
           .AddSingleton<IItemsRepository, InMemoryItemsRepository>();

        return services;
    }
}
