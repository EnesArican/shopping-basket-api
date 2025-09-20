using FluentAssertions;
using Moq;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Features.Basket.CreateBasket;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.UnitTests;

public class CreateBasketHandlerTests
{
    private readonly Mock<IBasketsRepository> _mockBasketRepository;
    private readonly CreateBasketHandler _handler;

    public CreateBasketHandlerTests()
    {
        _mockBasketRepository = new Mock<IBasketsRepository>();
        _handler = new CreateBasketHandler(_mockBasketRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsSuccess_ReturnsSuccessResult()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var expectedBasket = new Basket(basketId, new List<BasketItem>(), null);
        var repositoryResult = DataResult<Basket>.Success(expectedBasket);
        
        _mockBasketRepository
            .Setup(x => x.CreateBasketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var command = new CreateBasketCommand();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out var basket).Should().BeTrue();
        basket.Should().NotBeNull();
        basket!.Id.Should().Be(basketId);
        basket.Items.Should().BeEmpty();
        basket.DiscountCode.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsFailure_ReturnsFailureResult()
    {
        // Arrange
        const string expectedErrorCode = "REPOSITORY_ERROR";
        var repositoryResult = DataResult<Basket>.Failure(expectedErrorCode);
        
        _mockBasketRepository
            .Setup(x => x.CreateBasketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var command = new CreateBasketCommand();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().Be(expectedErrorCode);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsFailureWithNullErrorCode_ReturnsFailureResult()
    {
        // Arrange
        var repositoryResult = DataResult<Basket>.Failure(null);
        
        _mockBasketRepository
            .Setup(x => x.CreateBasketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var command = new CreateBasketCommand();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.ErrorCode.Should().BeNull();
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryReturnsSuccessButNullBasket_ReturnsFailureResult()
    {
        // Arrange
        var repositoryResult = new DataResult<Basket> { Data = null };
        
        _mockBasketRepository
            .Setup(x => x.CreateBasketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var command = new CreateBasketCommand();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.ExecuteAsync(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid(out _).Should().BeFalse();
        result.Data.Should().BeNull();
    }
}
