using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Products.GetProduct;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Products.TestData;

namespace SalesProject.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="GetProductHandler"/> class.
/// </summary>
public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetProductHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetProductHandler(_productRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get product request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When getting product Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = GetProductHandlerTestData.GenerateValidCommand();

        var product = GetProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        var result = GetProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<GetProductResult>(product)
            .Returns(result);

        // When
        var getProductResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getProductResult.Should().NotBeNull();
        getProductResult.Id.Should().Be(product.Id);
        getProductResult.Name.Should().Be(product.Name);
        getProductResult.CurrentPrice.Should().Be(product.CurrentPrice);
        getProductResult.Status.Should().Be(product.Status);
    }

    /// <summary>
    /// Tests that an invalid get product request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid product id When getting product Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new GetProductCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid product id When getting product Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new GetProductCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _productRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<GetProductResult>(Arg.Any<Product>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the product does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing product id When getting product Then throws key not found exception")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = GetProductHandlerTestData.GenerateValidCommand();

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Product with ID {command.Id} not found");

        await _productRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<GetProductResult>(Arg.Any<Product>());
    }

    /// <summary>
    /// Tests that the repository is called with the correct product id.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When getting product Then calls repository with correct id")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = GetProductHandlerTestData.GenerateValidCommand();

        var product = GetProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        var result = GetProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<GetProductResult>(product)
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1)
            .GetByIdAsync(
                Arg.Is<Guid>(id => id == command.Id),
                Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the product returned by repository.
    /// </summary>
    [Fact(DisplayName = "Given existing product When getting product Then maps product to result")]
    public async Task Handle_ValidRequest_MapsProductToResult()
    {
        // Given
        var command = GetProductHandlerTestData.GenerateValidCommand();

        var product = GetProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        var result = GetProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<GetProductResult>(product)
            .Returns(result);

        // When
        var getProductResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1)
            .Map<GetProductResult>(Arg.Is<Product>(p =>
                p.Id == product.Id &&
                p.Name == product.Name &&
                p.CurrentPrice == product.CurrentPrice &&
                p.Status == product.Status));

        getProductResult.Should().BeEquivalentTo(result);
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get the product.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenRepositoryFails_ThrowsException()
    {
        // Given
        var command = GetProductHandlerTestData.GenerateValidCommand();

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<Product?>>(_ => throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        _mapper.DidNotReceive()
            .Map<GetProductResult>(Arg.Any<Product>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the mapper fails to map the product.
    /// </summary>
    [Fact(DisplayName = "Given existing product When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = GetProductHandlerTestData.GenerateValidCommand();

        var product = GetProductHandlerTestData.GenerateProduct();
        product.Id = command.Id;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<GetProductResult>(product)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _productRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
}