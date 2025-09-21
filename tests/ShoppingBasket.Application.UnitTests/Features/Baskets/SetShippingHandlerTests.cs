using FluentAssertions;
using Moq;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Features.Baskets.SetShipping;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.UnitTests.Features.Baskets;

public class SetShippingHandlerTests
{
    private readonly Mock<IBasketsRepository> _mockBasketsRepository;
    private readonly SetShippingHandler _handler;

    public SetShippingHandlerTests()
    {
        _mockBasketsRepository = new Mock<IBasketsRepository>();
        _handler = new SetShippingHandler(_mockBasketsRepository.Object);
    }

    #region Success Tests

    [Fact]
    public async Task ExecuteAsync_WhenCountryIsUK_SetsUkShippingCost()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket(basketId, [], null);
        var command = new SetShippingCommand(basketId, "UK");

        _mockBasketsRepository.Setup(x => x.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));
        _mockBasketsRepository.Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        // Act
        var result = await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var updatedBasket).Should().BeTrue();
        updatedBasket.ShippingCountry.Should().Be("UK");
        updatedBasket.ShippingCost.Should().Be(5.99m);
    }

    [Theory]
    [InlineData("UK")]
    [InlineData("uk")]
    [InlineData("Uk")]
    [InlineData("uK")]
    public async Task ExecuteAsync_WhenCountryIsUkVariations_SetsUkShippingCost(string country)
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket(basketId, [], null);
        var command = new SetShippingCommand(basketId, country);

        _mockBasketsRepository.Setup(x => x.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));
        _mockBasketsRepository.Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        // Act
        var result = await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var updatedBasket).Should().BeTrue();
        updatedBasket.ShippingCountry.Should().Be(country);
        updatedBasket.ShippingCost.Should().Be(5.99m);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCountryIsInternational_SetsInternationalShippingCost()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket(basketId, [], null);
        var command = new SetShippingCommand(basketId, "France");

        _mockBasketsRepository.Setup(x => x.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));
        _mockBasketsRepository.Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        // Act
        var result = await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var updatedBasket).Should().BeTrue();
        updatedBasket.ShippingCountry.Should().Be("France");
        updatedBasket.ShippingCost.Should().Be(12.99m);
    }

    #endregion

    #region Error Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_WhenCountryIsInvalid_ReturnsInvalidShippingCountryError(string invalidCountry)
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var command = new SetShippingCommand(basketId, invalidCountry);

        // Act
        var result = await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidShippingCountry);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketNotFound_ReturnsBasketNotFoundError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var command = new SetShippingCommand(basketId, "UK");

        _mockBasketsRepository.Setup(x => x.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Failure(ErrorCodes.BasketNotFound));

        // Act
        var result = await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.BasketNotFound);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryUpdateFails_ReturnsRepositoryError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket(basketId, [], null);
        var command = new SetShippingCommand(basketId, "UK");

        _mockBasketsRepository.Setup(x => x.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));
        _mockBasketsRepository.Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Failure("repository_error"));

        // Act
        var result = await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be("repository_error");
    }

    #endregion

    #region Interaction Verification Tests

    [Fact]
    public async Task ExecuteAsync_WhenSuccessful_CallsRepositoryMethods()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket(basketId, [], null);
        var command = new SetShippingCommand(basketId, "UK");

        _mockBasketsRepository.Setup(x => x.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));
        _mockBasketsRepository.Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        // Act
        await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(It.Is<Basket>(b => 
            b.Id == basketId && 
            b.ShippingCountry == "UK" && 
            b.ShippingCost == 5.99m), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
