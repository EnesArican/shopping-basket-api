using FluentAssertions;
using Moq;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Features.Baskets.GetBasketTotal;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.UnitTests.Features.Baskets;

public class GetBasketTotalHandlerTests
{
    private readonly Mock<IBasketsRepository> _mockBasketsRepository;
    private readonly GetBasketTotalHandler _handler;

    public GetBasketTotalHandlerTests()
    {
        _mockBasketsRepository = new Mock<IBasketsRepository>();
        _handler = new GetBasketTotalHandler(_mockBasketsRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketExists_ReturnsCorrectTotals()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var item1 = new Item(Guid.NewGuid(), "Item 1", 10.00m);
        var item2 = new Item(Guid.NewGuid(), "Item 2", 5.00m);
        
        var basketItem1 = new BasketItem(Guid.NewGuid(), item1, 2, false, null);
        var basketItem2 = new BasketItem(Guid.NewGuid(), item2, 3, false, null);
        
        var basket = new Basket(basketId, new List<BasketItem> { basketItem1, basketItem2 }, null);
        
        _mockBasketsRepository
            .Setup(r => r.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        var query = new GetBasketTotalQuery(basketId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basketTotal).Should().BeTrue();
        basketTotal.BasketId.Should().Be(basketId);
        basketTotal.SubTotal.Should().Be(35.00m); // (10*2) + (5*3)
        basketTotal.VatAmount.Should().Be(7.00m); // 35 * 0.20
        basketTotal.TotalWithVat.Should().Be(42.00m); // 35 + 7
        basketTotal.TotalWithoutVat.Should().Be(35.00m);
        basketTotal.TotalItems.Should().Be(5); // 2 + 3
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketHasDiscountedItems_CalculatesDiscountsCorrectly()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var item = new Item(Guid.NewGuid(), "Discounted Item", 20.00m);
        
        // Item with 25% discount
        var basketItem = new BasketItem(Guid.NewGuid(), item, 1, true, 25);
        var basket = new Basket(basketId, new List<BasketItem> { basketItem }, null);
        
        _mockBasketsRepository
            .Setup(r => r.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        var query = new GetBasketTotalQuery(basketId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basketTotal).Should().BeTrue();
        basketTotal.BasketId.Should().Be(basketId);
        basketTotal.SubTotal.Should().Be(15.00m); // 20 - (20 * 0.25) = 15
        basketTotal.VatAmount.Should().Be(3.00m); // 15 * 0.20
        basketTotal.TotalWithVat.Should().Be(18.00m); // 15 + 3
        basketTotal.TotalWithoutVat.Should().Be(15.00m);
        basketTotal.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketHasVatItems_CalculatesVatCorrectly()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var item = new Item(Guid.NewGuid(), "VAT Item", 100.00m);
        
        var basketItem = new BasketItem(Guid.NewGuid(), item, 1, false, null);
        var basket = new Basket(basketId, new List<BasketItem> { basketItem }, null);
        
        _mockBasketsRepository
            .Setup(r => r.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        var query = new GetBasketTotalQuery(basketId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basketTotal).Should().BeTrue();
        basketTotal.BasketId.Should().Be(basketId);
        basketTotal.SubTotal.Should().Be(100.00m);
        basketTotal.VatAmount.Should().Be(20.00m); // 100 * 0.20 = 20% VAT
        basketTotal.TotalWithVat.Should().Be(120.00m); // 100 + 20
        basketTotal.TotalWithoutVat.Should().Be(100.00m);
        basketTotal.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketNotFound_ReturnsFailureResult()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        
        _mockBasketsRepository
            .Setup(r => r.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Failure(ErrorCodes.BasketNotFound));

        var query = new GetBasketTotalQuery(basketId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.BasketNotFound);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketIsEmpty_ReturnsZeroTotals()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket(basketId, new List<BasketItem>(), null);
        
        _mockBasketsRepository
            .Setup(r => r.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        var query = new GetBasketTotalQuery(basketId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basketTotal).Should().BeTrue();
        basketTotal.BasketId.Should().Be(basketId);
        basketTotal.SubTotal.Should().Be(0.00m);
        basketTotal.VatAmount.Should().Be(0.00m);
        basketTotal.TotalWithVat.Should().Be(0.00m);
        basketTotal.TotalWithoutVat.Should().Be(0.00m);
        basketTotal.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketHasShipping_IncludesShippingInTotals()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var item = new Item(Guid.NewGuid(), "Test Item", 10.00m);
        var basketItems = new List<BasketItem>
        {
            new(Guid.NewGuid(), item, 2, false, null)
        };
        var basket = new Basket(basketId, basketItems, null, "UK", 5.99m);

        _mockBasketsRepository.Setup(x => x.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        var query = new GetBasketTotalQuery(basketId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basketTotal).Should().BeTrue();
        basketTotal.BasketId.Should().Be(basketId);
        basketTotal.SubTotal.Should().Be(20.00m); // 2 items * £10
        basketTotal.ShippingCost.Should().Be(5.99m); // UK shipping
        basketTotal.TotalWithoutVat.Should().Be(25.99m); // SubTotal + Shipping
        basketTotal.VatAmount.Should().Be(5.198m); // 20% of (SubTotal + Shipping)
        basketTotal.TotalWithVat.Should().Be(31.188m); // TotalWithoutVat + VAT
        basketTotal.TotalItems.Should().Be(2);
    }
}
