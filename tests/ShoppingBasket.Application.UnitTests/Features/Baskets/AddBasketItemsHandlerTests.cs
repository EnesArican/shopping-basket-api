using FluentAssertions;
using Moq;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Features.Basket.AddBasketItems;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.UnitTests.Features.Baskets;

public class AddBasketItemsHandlerTests
{
    private readonly Mock<IBasketsRepository> _mockBasketsRepository;
    private readonly Mock<IItemsRepository> _mockItemsRepository;
    private readonly AddBasketItemsHandler _handler;

    public AddBasketItemsHandlerTests()
    {
        _mockBasketsRepository = new Mock<IBasketsRepository>();
        _mockItemsRepository = new Mock<IItemsRepository>();
        _handler = new AddBasketItemsHandler(_mockBasketsRepository.Object, _mockItemsRepository.Object);
    }

    #region Validation Tests

    [Fact]
    public async Task ExecuteAsync_WhenItemsListIsNull_ReturnsInvalidRequestError()
    {
        // Arrange
        var command = new AddBasketItemsCommand(Guid.NewGuid(), null!);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidRequest);
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemsListIsEmpty_ReturnsInvalidRequestError()
    {
        // Arrange
        var command = new AddBasketItemsCommand(Guid.NewGuid(), new List<BasketItemRequest>());
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidRequest);
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemQuantityIsZero_ReturnsInvalidQuantityError()
    {
        // Arrange
        var basketItemRequest = new BasketItemRequest(Guid.NewGuid(), 0, false, null);
        var command = new AddBasketItemsCommand(Guid.NewGuid(), new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidQuantity);
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemQuantityIsNegative_ReturnsInvalidQuantityError()
    {
        // Arrange
        var basketItemRequest = new BasketItemRequest(Guid.NewGuid(), -1, false, null);
        var command = new AddBasketItemsCommand(Guid.NewGuid(), new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidQuantity);
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemIsDiscountedButDiscountPercentageIsNull_ReturnsInvalidDiscountPercentageError()
    {
        // Arrange
        var basketItemRequest = new BasketItemRequest(Guid.NewGuid(), 1, true, null);
        var command = new AddBasketItemsCommand(Guid.NewGuid(), new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidDiscountPercentage);
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemIsDiscountedButDiscountPercentageIsZero_ReturnsInvalidDiscountPercentageError()
    {
        // Arrange
        var basketItemRequest = new BasketItemRequest(Guid.NewGuid(), 1, true, 0);
        var command = new AddBasketItemsCommand(Guid.NewGuid(), new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidDiscountPercentage);
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemIsDiscountedButDiscountPercentageIsNegative_ReturnsInvalidDiscountPercentageError()
    {
        // Arrange
        var basketItemRequest = new BasketItemRequest(Guid.NewGuid(), 1, true, -5);
        var command = new AddBasketItemsCommand(Guid.NewGuid(), new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidDiscountPercentage);
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemIsDiscountedButDiscountPercentageIsOver100_ReturnsInvalidDiscountPercentageError()
    {
        // Arrange
        var basketItemRequest = new BasketItemRequest(Guid.NewGuid(), 1, true, 150);
        var command = new AddBasketItemsCommand(Guid.NewGuid(), new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidDiscountPercentage);
    }

    #endregion

    #region Repository Error Tests

    [Fact]
    public async Task ExecuteAsync_WhenBasketNotFound_ReturnsBasketNotFoundError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basketItemRequest = new BasketItemRequest(Guid.NewGuid(), 1, false, null);
        var command = new AddBasketItemsCommand(basketId, new List<BasketItemRequest> { basketItemRequest });
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
    }

    [Fact]
    public async Task ExecuteAsync_WhenItemNotFoundInCatalog_ReturnsItemNotFoundError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basketItemRequest = new BasketItemRequest(itemId, 1, false, null);
        var command = new AddBasketItemsCommand(basketId, new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        var existingBasket = new Basket(basketId, new List<BasketItem>(), null);
        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Failure(ErrorCodes.ItemNotFound));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ItemNotFound);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBasketsRepositoryUpdateFails_ReturnsRepositoryError()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basketItemRequest = new BasketItemRequest(itemId, 1, false, null);
        var command = new AddBasketItemsCommand(basketId, new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        var existingBasket = new Basket(basketId, new List<BasketItem>(), null);
        var catalogItem = new Item(itemId, "Test Item", 10.00m);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Failure(ErrorCodes.ServerError));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ServerError);
    }

    #endregion

    #region Business Logic Tests - New Items

    [Fact]
    public async Task ExecuteAsync_WhenAddingSingleNewItem_ReturnsUpdatedBasketWithNewItem()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basketItemRequest = new BasketItemRequest(itemId, 2, false, null);
        var command = new AddBasketItemsCommand(basketId, new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        var existingBasket = new Basket(basketId, new List<BasketItem>(), null);
        var catalogItem = new Item(itemId, "Test Item", 10.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem>
        {
            new BasketItem(Guid.NewGuid(), catalogItem, 2, false, null, 20.00m)
        }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(1);
        basket.Items.First().Item.Id.Should().Be(itemId);
        basket.Items.First().Quantity.Should().Be(2);
        basket.Items.First().IsDiscounted.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_WhenAddingMultipleNewItems_ReturnsUpdatedBasketWithAllNewItems()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId1 = Guid.NewGuid();
        var itemId2 = Guid.NewGuid();
        var basketItemRequests = new List<BasketItemRequest>
        {
            new BasketItemRequest(itemId1, 1, false, null),
            new BasketItemRequest(itemId2, 3, false, null)
        };
        var command = new AddBasketItemsCommand(basketId, basketItemRequests);
        var cancellationToken = CancellationToken.None;

        var existingBasket = new Basket(basketId, new List<BasketItem>(), null);
        var catalogItem1 = new Item(itemId1, "Test Item 1", 5.00m);
        var catalogItem2 = new Item(itemId2, "Test Item 2", 15.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem>
        {
            new BasketItem(Guid.NewGuid(), catalogItem1, 1, false, null, 5.00m),
            new BasketItem(Guid.NewGuid(), catalogItem2, 3, false, null, 45.00m)
        }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId1, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem1));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId2, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem2));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(2);
        basket.Items.Should().Contain(i => i.Item.Id == itemId1 && i.Quantity == 1);
        basket.Items.Should().Contain(i => i.Item.Id == itemId2 && i.Quantity == 3);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAddingNewItemWithDiscount_ReturnsUpdatedBasketWithDiscountedItem()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basketItemRequest = new BasketItemRequest(itemId, 1, true, 25);
        var command = new AddBasketItemsCommand(basketId, new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        var existingBasket = new Basket(basketId, new List<BasketItem>(), null);
        var catalogItem = new Item(itemId, "Test Item", 20.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem>
        {
            new BasketItem(Guid.NewGuid(), catalogItem, 1, true, 25, 20.00m)
        }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(1);
        basket.Items.First().IsDiscounted.Should().BeTrue();
        basket.Items.First().DiscountPercentage.Should().Be(25);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAddingNewItemWithoutDiscount_ReturnsUpdatedBasketWithRegularItem()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basketItemRequest = new BasketItemRequest(itemId, 1, false, null);
        var command = new AddBasketItemsCommand(basketId, new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        var existingBasket = new Basket(basketId, new List<BasketItem>(), null);
        var catalogItem = new Item(itemId, "Test Item", 10.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem>
        {
            new BasketItem(Guid.NewGuid(), catalogItem, 1, false, null, 10.00m)
        }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(1);
        basket.Items.First().IsDiscounted.Should().BeFalse();
        basket.Items.First().DiscountPercentage.Should().BeNull();
    }

    #endregion

    #region Business Logic Tests - Existing Items

    [Fact]
    public async Task ExecuteAsync_WhenUpdatingExistingItem_ReturnsUpdatedBasketWithIncreasedQuantity()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basketItemRequest = new BasketItemRequest(itemId, 2, false, null);
        var command = new AddBasketItemsCommand(basketId, new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        var catalogItem = new Item(itemId, "Test Item", 10.00m);
        var existingBasketItem = new BasketItem(Guid.NewGuid(), catalogItem, 1, false, null, 10.00m);
        var existingBasket = new Basket(basketId, new List<BasketItem> { existingBasketItem }, null);
        
        var updatedBasketItem = new BasketItem(existingBasketItem.Id, catalogItem, 3, false, null, 30.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem> { updatedBasketItem }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(1);
        basket.Items.First().Quantity.Should().Be(3); // 1 existing + 2 added
        basket.Items.First().TotalPrice.Should().Be(30.00m);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUpdatingExistingItemDiscount_ReturnsUpdatedBasketWithUpdatedDiscount()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basketItemRequest = new BasketItemRequest(itemId, 1, true, 15);
        var command = new AddBasketItemsCommand(basketId, new List<BasketItemRequest> { basketItemRequest });
        var cancellationToken = CancellationToken.None;

        var catalogItem = new Item(itemId, "Test Item", 20.00m);
        var existingBasketItem = new BasketItem(Guid.NewGuid(), catalogItem, 2, false, null, 40.00m);
        var existingBasket = new Basket(basketId, new List<BasketItem> { existingBasketItem }, null);
        
        var updatedBasketItem = new BasketItem(existingBasketItem.Id, catalogItem, 3, true, 15, 60.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem> { updatedBasketItem }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(1);
        basket.Items.First().Quantity.Should().Be(3); // 2 existing + 1 added
        basket.Items.First().IsDiscounted.Should().BeTrue();
        basket.Items.First().DiscountPercentage.Should().Be(15);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUpdatingMultipleExistingItems_ReturnsUpdatedBasketWithAllUpdatedItems()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId1 = Guid.NewGuid();
        var itemId2 = Guid.NewGuid();
        var basketItemRequests = new List<BasketItemRequest>
        {
            new BasketItemRequest(itemId1, 1, false, null),
            new BasketItemRequest(itemId2, 2, true, 10)
        };
        var command = new AddBasketItemsCommand(basketId, basketItemRequests);
        var cancellationToken = CancellationToken.None;

        var catalogItem1 = new Item(itemId1, "Test Item 1", 5.00m);
        var catalogItem2 = new Item(itemId2, "Test Item 2", 15.00m);
        var existingBasketItem1 = new BasketItem(Guid.NewGuid(), catalogItem1, 2, false, null, 10.00m);
        var existingBasketItem2 = new BasketItem(Guid.NewGuid(), catalogItem2, 1, false, null, 15.00m);
        var existingBasket = new Basket(basketId, new List<BasketItem> { existingBasketItem1, existingBasketItem2 }, null);
        
        var updatedBasketItem1 = new BasketItem(existingBasketItem1.Id, catalogItem1, 3, false, null, 15.00m);
        var updatedBasketItem2 = new BasketItem(existingBasketItem2.Id, catalogItem2, 3, true, 10, 45.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem> { updatedBasketItem1, updatedBasketItem2 }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId1, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem1));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId2, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem2));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(2);
        
        var item1 = basket.Items.First(i => i.Item.Id == itemId1);
        item1.Quantity.Should().Be(3); // 2 existing + 1 added
        item1.IsDiscounted.Should().BeFalse();
        
        var item2 = basket.Items.First(i => i.Item.Id == itemId2);
        item2.Quantity.Should().Be(3); // 1 existing + 2 added
        item2.IsDiscounted.Should().BeTrue();
        item2.DiscountPercentage.Should().Be(10);
    }

    #endregion

    #region Business Logic Tests - Mixed Scenarios

    [Fact]
    public async Task ExecuteAsync_WhenAddingMixOfNewAndExistingItems_ReturnsUpdatedBasketWithAllChanges()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var existingItemId = Guid.NewGuid();
        var newItemId = Guid.NewGuid();
        var basketItemRequests = new List<BasketItemRequest>
        {
            new BasketItemRequest(existingItemId, 2, false, null), // Update existing
            new BasketItemRequest(newItemId, 1, true, 20)         // Add new
        };
        var command = new AddBasketItemsCommand(basketId, basketItemRequests);
        var cancellationToken = CancellationToken.None;

        var existingCatalogItem = new Item(existingItemId, "Existing Item", 10.00m);
        var newCatalogItem = new Item(newItemId, "New Item", 25.00m);
        var existingBasketItem = new BasketItem(Guid.NewGuid(), existingCatalogItem, 1, false, null, 10.00m);
        var existingBasket = new Basket(basketId, new List<BasketItem> { existingBasketItem }, null);
        
        var updatedExistingItem = new BasketItem(existingBasketItem.Id, existingCatalogItem, 3, false, null, 30.00m);
        var newBasketItem = new BasketItem(Guid.NewGuid(), newCatalogItem, 1, true, 20, 25.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem> { updatedExistingItem, newBasketItem }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(existingItemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(existingCatalogItem));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(newItemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(newCatalogItem));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(2);
        
        var existingItem = basket.Items.First(i => i.Item.Id == existingItemId);
        existingItem.Quantity.Should().Be(3); // 1 existing + 2 added
        existingItem.IsDiscounted.Should().BeFalse();
        
        var newItem = basket.Items.First(i => i.Item.Id == newItemId);
        newItem.Quantity.Should().Be(1);
        newItem.IsDiscounted.Should().BeTrue();
        newItem.DiscountPercentage.Should().Be(20);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAddingSameItemMultipleTimes_ReturnsUpdatedBasketWithCombinedQuantity()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var basketItemRequests = new List<BasketItemRequest>
        {
            new BasketItemRequest(itemId, 2, false, null),
            new BasketItemRequest(itemId, 3, true, 15) // Same item, different properties
        };
        var command = new AddBasketItemsCommand(basketId, basketItemRequests);
        var cancellationToken = CancellationToken.None;

        var existingBasket = new Basket(basketId, new List<BasketItem>(), null);
        var catalogItem = new Item(itemId, "Test Item", 10.00m);
        
        // After processing both requests, the item should have quantity = 2 + 3 = 5
        // Last request wins for discount properties
        var finalBasketItem = new BasketItem(Guid.NewGuid(), catalogItem, 5, true, 15, 50.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem> { finalBasketItem }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(itemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(1);
        basket.Items.First().Quantity.Should().Be(5); // Combined quantity
        basket.Items.First().IsDiscounted.Should().BeTrue(); // Last request wins
        basket.Items.First().DiscountPercentage.Should().Be(15); // Last request wins
    }

    [Fact]
    public async Task ExecuteAsync_WhenProcessingComplexMixedRequest_ReturnsCorrectlyUpdatedBasket()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var existingItemId1 = Guid.NewGuid();
        var existingItemId2 = Guid.NewGuid();
        var newItemId = Guid.NewGuid();
        
        var basketItemRequests = new List<BasketItemRequest>
        {
            new BasketItemRequest(existingItemId1, 1, false, null),    // Update existing item 1
            new BasketItemRequest(newItemId, 2, true, 30),             // Add new item
            new BasketItemRequest(existingItemId2, 3, true, 10),       // Update existing item 2
            new BasketItemRequest(existingItemId1, 2, true, 25)        // Update existing item 1 again
        };
        var command = new AddBasketItemsCommand(basketId, basketItemRequests);
        var cancellationToken = CancellationToken.None;

        var catalogItem1 = new Item(existingItemId1, "Existing Item 1", 5.00m);
        var catalogItem2 = new Item(existingItemId2, "Existing Item 2", 15.00m);
        var catalogItem3 = new Item(newItemId, "New Item", 20.00m);
        
        var existingBasketItem1 = new BasketItem(Guid.NewGuid(), catalogItem1, 2, false, null, 10.00m);
        var existingBasketItem2 = new BasketItem(Guid.NewGuid(), catalogItem2, 1, false, null, 15.00m);
        var existingBasket = new Basket(basketId, new List<BasketItem> { existingBasketItem1, existingBasketItem2 }, null);
        
        // Expected results after processing all requests:
        // Item 1: 2 + 1 + 2 = 5 quantity, discounted 25% (last request wins)
        // Item 2: 1 + 3 = 4 quantity, discounted 10%
        // Item 3: 2 quantity, discounted 30%
        var updatedBasketItem1 = new BasketItem(existingBasketItem1.Id, catalogItem1, 5, true, 25, 25.00m);
        var updatedBasketItem2 = new BasketItem(existingBasketItem2.Id, catalogItem2, 4, true, 10, 60.00m);
        var newBasketItem = new BasketItem(Guid.NewGuid(), catalogItem3, 2, true, 30, 40.00m);
        var updatedBasket = new Basket(basketId, new List<BasketItem> { updatedBasketItem1, updatedBasketItem2, newBasketItem }, null);

        _mockBasketsRepository
            .Setup(x => x.GetBasketByIdAsync(basketId, cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(existingBasket));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(existingItemId1, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem1));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(existingItemId2, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem2));

        _mockItemsRepository
            .Setup(x => x.GetItemByIdAsync(newItemId, cancellationToken))
            .ReturnsAsync(DataResult<Item>.Success(catalogItem3));

        _mockBasketsRepository
            .Setup(x => x.UpdateBasketAsync(It.IsAny<Basket>(), cancellationToken))
            .ReturnsAsync(DataResult<Basket>.Success(updatedBasket));

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Items.Should().HaveCount(3);
        
        var item1 = basket.Items.First(i => i.Item.Id == existingItemId1);
        item1.Quantity.Should().Be(5);
        item1.IsDiscounted.Should().BeTrue();
        item1.DiscountPercentage.Should().Be(25);
        
        var item2 = basket.Items.First(i => i.Item.Id == existingItemId2);
        item2.Quantity.Should().Be(4);
        item2.IsDiscounted.Should().BeTrue();
        item2.DiscountPercentage.Should().Be(10);
        
        var item3 = basket.Items.First(i => i.Item.Id == newItemId);
        item3.Quantity.Should().Be(2);
        item3.IsDiscounted.Should().BeTrue();
        item3.DiscountPercentage.Should().Be(30);
    }

    #endregion
}
