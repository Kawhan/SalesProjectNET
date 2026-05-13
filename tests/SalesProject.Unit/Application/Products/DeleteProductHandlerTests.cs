using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Products.DeleteProduct;
using SalesProject.Application.Products.Events;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Products.TestData;

namespace SalesProject.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="DeleteProductHandler"/> class.
/// </summary>
public class DeleteProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMessageBusService _messageBusService;
    private readonly DeleteProductHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProductHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public DeleteProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new DeleteProductHandler(_productRepository, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid product delete request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When deleting product Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = DeleteProductHandlerTestData.GenerateValidCommand();

        var product = DeleteProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        await _productRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid product delete request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid product id When deleting product Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new DeleteProductCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid product id When deleting product Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new DeleteProductCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _productRepository.DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the product does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing product id When deleting product Then throws key not found exception")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = DeleteProductHandlerTestData.GenerateValidCommand();

        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Product with ID {command.Id} not found");

        await _productRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the repository is called with the correct product id.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When deleting product Then calls repository with correct id")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = DeleteProductHandlerTestData.GenerateValidCommand();

        var product = DeleteProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).DeleteAsync(
            Arg.Is<Guid>(id => id == command.Id),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the deleted product is retrieved after successful delete.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When deleting product Then gets deleted product data")]
    public async Task Handle_ValidRequest_GetsDeletedProductData()
    {
        // Given
        var command = DeleteProductHandlerTestData.GenerateValidCommand();

        var product = DeleteProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a ProductDeletedEvent is published after deleting the product.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When product is deleted Then publishes product deleted event")]
    public async Task Handle_ValidRequest_PublishesProductDeletedEvent()
    {
        // Given
        var command = DeleteProductHandlerTestData.GenerateValidCommand();

        var product = DeleteProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<ProductDeletedEvent>(e =>
                e.Id == product.Id &&
                e.Name == product.Name &&
                e.CurrentPrice == product.CurrentPrice &&
                e.Status == product.Status &&
                e.CreatedAt == product.CreatedAt &&
                e.UpdatedAt == product.UpdatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to delete the product.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When delete repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenDeleteRepositoryFails_ThrowsException()
    {
        // Given
        var command = DeleteProductHandlerTestData.GenerateValidCommand();

        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<bool>>(_ => throw new InvalidOperationException("Repository delete error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository delete error");

        await _productRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get the deleted product.
    /// </summary>
    [Fact(DisplayName = "Given deleted product When get by id fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenGetByIdFails_ThrowsException()
    {
        // Given
        var command = DeleteProductHandlerTestData.GenerateValidCommand();

        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<Product?>>(_ => throw new InvalidOperationException("Repository get error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository get error");

        await _productRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails to publish the deleted event.
    /// </summary>
    [Fact(DisplayName = "Given deleted product When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = DeleteProductHandlerTestData.GenerateValidCommand();

        var product = DeleteProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _messageBusService
            .PublishAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _productRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _productRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
}