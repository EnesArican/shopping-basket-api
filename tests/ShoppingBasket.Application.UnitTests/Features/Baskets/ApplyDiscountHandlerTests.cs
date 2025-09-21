using FluentAssertions;
using Moq;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Features.Baskets.ApplyDiscount;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.UnitTests.Features.Baskets;

public class ApplyDiscountHandlerTests
{
    private readonly Mock<IBasketsRepository> _mockBasketsRepository;
    private readonly ApplyDiscountHandler _handler;

    public ApplyDiscountHandlerTests()
    {
        _mockBasketsRepository = new Mock<IBasketsRepository>();
        _handler = new ApplyDiscountHandler(_mockBasketsRepository.Object);
    }

    #region Validation Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_WhenDiscountCodeIsInvalid_ReturnsInvalidDiscountCodeError(string invalidDiscountCode)
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var command = new ApplyDiscountCommand(basketId, invalidDiscountCode);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidDiscountCode);
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketNotFound_ReturnsBasketNotFoundError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var command = new ApplyDiscountCommand(basketId, "SAVE20");
        var cancellationToken = CancellationToken.None;

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Failure(ErrorCodes.BasketNotFound));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.BasketNotFound);
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, cancellationToken), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDiscountCodeFormatIsInvalid_ReturnsInvalidDiscountCodeError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var invalidCode = "INVALID123";
        var command = new ApplyDiscountCommand(basketId, invalidCode);
        var cancellationToken = CancellationToken.None;

        var basket = new Basket(basketId, new List<BasketItem>(), null);
        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidDiscountCode);
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, cancellationToken), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Success Tests

    [Fact]
    public async Task ExecuteAsync_WhenValidDiscountCodeProvided_AppliesDiscountSuccessfully()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var discountCode = "SAVE20";
        var command = new ApplyDiscountCommand(basketId, discountCode);
        var cancellationToken = CancellationToken.None;

        var basket = new Basket(basketId, new List<BasketItem>(), null);
        var updatedBasket = new Basket(basketId, new List<BasketItem>(), discountCode);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.IsValid(out _).Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.DiscountCode.Should().Be(discountCode);

        // Verify repository interactions
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, cancellationToken), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(
            It.Is<Basket>(b => b.Id == basketId && b.DiscountCode == discountCode), 
            cancellationToken), Times.Once);
    }

    #endregion

    #region Case Sensitivity Tests

    [Theory]
    [InlineData("save20")]
    [InlineData("Save20")]
    public async Task ExecuteAsync_WhenDiscountCodeHasDifferentCasing_AppliesDiscountSuccessfully(string discountCode)
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var command = new ApplyDiscountCommand(basketId, discountCode);
        var cancellationToken = CancellationToken.None;

        var basket = new Basket(basketId, new List<BasketItem>(), null);
        var updatedBasket = new Basket(basketId, new List<BasketItem>(), discountCode);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.IsValid(out _).Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.DiscountCode.Should().Be(discountCode);
    }

    #endregion

    #region Multiple Valid Codes Tests

    [Theory]
    [InlineData("WINTER15")]
    [InlineData("STUDENT10")]
    [InlineData("VIP30")]
    public async Task ExecuteAsync_WhenUsingValidDiscountCodes_AppliesDiscountSuccessfully(string discountCode)
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var command = new ApplyDiscountCommand(basketId, discountCode);
        var cancellationToken = CancellationToken.None;

        var basket = new Basket(basketId, new List<BasketItem>(), null);
        var updatedBasket = new Basket(basketId, new List<BasketItem>(), discountCode);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.IsValid(out _).Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.DiscountCode.Should().Be(discountCode);
    }

    #endregion
}
