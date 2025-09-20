using FluentAssertions;
using Moq;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Features.Baskets.RemoveBasketItem;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.UnitTests.Features.Baskets;

public class RemoveBasketItemHandlerTests
{
    private readonly Mock<IBasketsRepository> _mockBasketsRepository;
    private readonly RemoveBasketItemHandler _handler;

    public RemoveBasketItemHandlerTests()
    {
        _mockBasketsRepository = new Mock<IBasketsRepository>();
        _handler = new RemoveBasketItemHandler(_mockBasketsRepository.Object);
    }

    #region Error/Validation Tests

    [Fact]
    public async Task ExecuteAsync_WhenBasketNotFound_ReturnsBasketNotFoundError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new RemoveBasketItemCommand(basketId, itemId);
        var cancellationToken = CancellationToken.None;

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Failure(ErrorCodes.BasketNotFound));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.BasketNotFound);
        
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, cancellationToken), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemNotFoundInBasket_ReturnsItemNotFoundError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var existingItemId = Guid.NewGuid();
        var command = new RemoveBasketItemCommand(basketId, itemId);
        var cancellationToken = CancellationToken.None;

        var existingItem = new Item(existingItemId, "Laptop", 999.99m);
        var basketItem = new BasketItem(Guid.NewGuid(), existingItem, 1, false, null);
        var basket = new Basket(basketId, [basketItem], null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ItemNotFound);
        
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, cancellationToken), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketsRepositoryUpdateFails_ReturnsRepositoryError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new RemoveBasketItemCommand(basketId, itemId);
        var cancellationToken = CancellationToken.None;

        var item = new Item(itemId, "Laptop", 999.99m);
        var basketItem = new BasketItem(Guid.NewGuid(), item, 1, false, null);
        var basket = new Basket(basketId, [basketItem], null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Failure(ErrorCodes.ServerError));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ServerError);
        
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, cancellationToken), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken), Times.Once);
    }

    #endregion

    #region Success Tests

    [Fact]
    public async Task ExecuteAsync_WhenRemovingExistingItem_ReturnsUpdatedBasketWithoutItem()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new RemoveBasketItemCommand(basketId, itemId);
        var cancellationToken = CancellationToken.None;

        var item = new Item(itemId, "Laptop", 999.99m);
        var basketItem = new BasketItem(Guid.NewGuid(), item, 2, false, null);
        var basket = new Basket(basketId, [basketItem], null);

        var updatedBasket = new Basket(basketId, [], null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var resultBasket).Should().BeTrue();
        resultBasket.Should().NotBeNull();
        resultBasket!.Id.Should().Be(basketId);
        resultBasket.Items.Should().BeEmpty();
        
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, cancellationToken), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRemovingItemFromBasketWithMultipleItems_ReturnsUpdatedBasketWithRemainingItems()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemToRemoveId = Guid.NewGuid();
        var remainingItemId = Guid.NewGuid();
        var command = new RemoveBasketItemCommand(basketId, itemToRemoveId);
        var cancellationToken = CancellationToken.None;

        var itemToRemove = new Item(itemToRemoveId, "Laptop", 999.99m);
        var remainingItem = new Item(remainingItemId, "Mouse", 29.99m);
        
        var basketItemToRemove = new BasketItem(Guid.NewGuid(), itemToRemove, 1, false, null);
        var basketItemToKeep = new BasketItem(Guid.NewGuid(), remainingItem, 2, false, null);
        
        var basket = new Basket(basketId, [basketItemToRemove, basketItemToKeep], null);
        var updatedBasket = new Basket(basketId, [basketItemToKeep], null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(basket));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var resultBasket).Should().BeTrue();
        resultBasket.Should().NotBeNull();
        resultBasket!.Id.Should().Be(basketId);
        resultBasket.Items.Should().HaveCount(1);
        resultBasket.Items.First().Item.Id.Should().Be(remainingItemId);
        resultBasket.Items.Should().NotContain(i => i.Item.Id == itemToRemoveId);
        
        _mockBasketsRepository.Verify(x => x.GetBasketByIdAsync(basketId, cancellationToken), Times.Once);
        _mockBasketsRepository.Verify(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken), Times.Once);
    }

    #endregion
}
