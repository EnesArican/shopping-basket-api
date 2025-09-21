using FluentAssertions;
using Moq;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Features.Items.GetItems;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.UnitTests.Features.Items;

public class GetItemsHandlerTests
{
    private readonly Mock<IItemsRepository> _mockItemRepository;
    private readonly GetItemsHandler _handler;

    public GetItemsHandlerTests()
    {
        _mockItemRepository = new Mock<IItemsRepository>();
        _handler = new GetItemsHandler(_mockItemRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsSuccess_ReturnsSuccessResult()
    {
        // Arrange
        var expectedItems = new List<Item>
        {
            new(Guid.NewGuid(), "Laptop", 1000.00m),
            new(Guid.NewGuid(), "Mouse", 30.00m),
            new(Guid.NewGuid(), "Keyboard", 150.00m)
        };
        var repositoryResult = DataResult<List<Item>>.Success(expectedItems);
        
        _mockItemRepository
            .Setup(x => x.GetAllItemsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var query = new GetItemsQuery();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var items).Should().BeTrue();
        items.Should().NotBeNull();
        items!.Should().HaveCount(3);
        items.Should().Contain(item => item.Name == "Laptop" && item.Price == 1000.00m);
        items.Should().Contain(item => item.Name == "Mouse" && item.Price == 30.00m);
        items.Should().Contain(item => item.Name == "Keyboard" && item.Price == 150.00m);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsEmptyList_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        var expectedItems = new List<Item>();
        var repositoryResult = DataResult<List<Item>>.Success(expectedItems);
        
        _mockItemRepository
            .Setup(x => x.GetAllItemsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var query = new GetItemsQuery();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var items).Should().BeTrue();
        items.Should().NotBeNull();
        items!.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsFailure_ReturnsFailureResult()
    {
        // Arrange
        const string expectedErrorCode = "REPOSITORY_ERROR";
        var repositoryResult = DataResult<List<Item>>.Failure(expectedErrorCode);
        
        _mockItemRepository
            .Setup(x => x.GetAllItemsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var query = new GetItemsQuery();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(expectedErrorCode);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsServerError_ReturnsServerErrorResult()
    {
        // Arrange
        const string expectedErrorCode = ErrorCodes.ServerError;
        var repositoryResult = DataResult<List<Item>>.Failure(expectedErrorCode);
        
        _mockItemRepository
            .Setup(x => x.GetAllItemsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var query = new GetItemsQuery();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ServerError);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsFailureWithNullErrorCode_ReturnsFailureResult()
    {
        // Arrange
        var repositoryResult = DataResult<List<Item>>.Failure(null);
        
        _mockItemRepository
            .Setup(x => x.GetAllItemsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var query = new GetItemsQuery();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().BeNull();
        result.Data.Should().BeNull();
    }
}
