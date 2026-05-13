using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Products.CreateProduct;
using SalesProject.Application.Products.Events;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Products.TestData;

namespace SalesProject.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="CreateProductHandler"/> class.
/// </summary>
public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;
    private readonly CreateProductHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new CreateProductHandler(_productRepository, _mapper, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid product creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When creating product Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        var result = CreateProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<CreateProductResult>(product)
            .Returns(result);

        // When
        var createProductResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createProductResult.Should().NotBeNull();
        createProductResult.Id.Should().Be(product.Id);
        createProductResult.Name.Should().Be(product.Name);
        createProductResult.CurrentPrice.Should().Be(product.CurrentPrice);
        createProductResult.Status.Should().Be(product.Status);

        await _productRepository.Received(1)
            .CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid product creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid product data When creating product Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateProductCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid product data When creating product Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new CreateProductCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _productRepository.DidNotReceive()
            .GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<Product>(Arg.Any<CreateProductCommand>());

        _mapper.DidNotReceive()
            .Map<CreateProductResult>(Arg.Any<Product>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when a product with the same name already exists.
    /// </summary>
    [Fact(DisplayName = "Given existing product name When creating product Then throws invalid operation exception")]
    public async Task Handle_ProductAlreadyExists_ThrowsInvalidOperationException()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = CreateProductHandlerTestData.GenerateProduct();
        existingProduct.Name = command.Name;

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"Product with name {command.Name} already exists");

        await _productRepository.Received(1)
            .GetByNameAsync(command.Name, Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<Product>(Arg.Any<CreateProductCommand>());

        await _productRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the repository checks if a product with the same name already exists.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When creating product Then checks product by name")]
    public async Task Handle_ValidRequest_ChecksProductByName()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        var result = CreateProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<CreateProductResult>(product)
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1)
            .GetByNameAsync(
                Arg.Is<string>(name => name == command.Name),
                Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct command.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps command to product entity")]
    public async Task Handle_ValidRequest_MapsCommandToProduct()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        var result = CreateProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<CreateProductResult>(product)
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<Product>(Arg.Is<CreateProductCommand>(c =>
            c.Name == command.Name &&
            c.CurrentPrice == command.CurrentPrice &&
            c.Status == command.Status));
    }

    /// <summary>
    /// Tests that the product repository is called with the correct product entity.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then creates product in repository")]
    public async Task Handle_ValidRequest_CreatesProductInRepository()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        var result = CreateProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<CreateProductResult>(product)
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).CreateAsync(
            Arg.Is<Product>(p =>
                p.Name == command.Name &&
                p.CurrentPrice == command.CurrentPrice &&
                p.Status == command.Status),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a ProductCreatedEvent is published after creating the product.
    /// </summary>
    [Fact(DisplayName = "Given valid command When product is created Then publishes product created event")]
    public async Task Handle_ValidRequest_PublishesProductCreatedEvent()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        var result = CreateProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<CreateProductResult>(product)
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<ProductCreatedEvent>(e =>
                e.Id == product.Id &&
                e.Name == product.Name &&
                e.CurrentPrice == product.CurrentPrice &&
                e.Status == product.Status &&
                e.CreatedAt == product.CreatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the created product is mapped to result.
    /// </summary>
    [Fact(DisplayName = "Given created product When handling Then maps created product to result")]
    public async Task Handle_ValidRequest_MapsCreatedProductToResult()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        var result = CreateProductHandlerTestData.GenerateProductResult(product);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<CreateProductResult>(product)
            .Returns(result);

        // When
        var createProductResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1)
            .Map<CreateProductResult>(Arg.Is<Product>(p =>
                p.Id == product.Id &&
                p.Name == product.Name &&
                p.CurrentPrice == product.CurrentPrice &&
                p.Status == product.Status));

        createProductResult.Should().BeEquivalentTo(result);
    }

    /// <summary>
    /// Tests that an exception is thrown when GetByNameAsync fails.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When get by name repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenGetByNameRepositoryFails_ThrowsException()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns<Task<Product?>>(_ => throw new InvalidOperationException("Repository get by name error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository get by name error");

        _mapper.DidNotReceive()
            .Map<Product>(Arg.Any<CreateProductCommand>());

        await _productRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the command cannot be mapped to a product entity.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _productRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to create the product.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When repository create fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenCreateRepositoryFails_ThrowsException()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns<Task<Product>>(_ => throw new InvalidOperationException("Repository create error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository create error");

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductCreatedEvent>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<CreateProductResult>(Arg.Any<Product>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails to publish the created event.
    /// </summary>
    [Fact(DisplayName = "Given created product When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _messageBusService
            .PublishAsync(Arg.Any<ProductCreatedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _productRepository.Received(1)
            .CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<CreateProductResult>(Arg.Any<Product>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the created product cannot be mapped to result.
    /// </summary>
    [Fact(DisplayName = "Given created product When result mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenResultMapperFails_ThrowsException()
    {
        // Given
        var command = CreateProductHandlerTestData.GenerateValidCommand();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(command)
            .Returns(product);

        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<CreateProductResult>(product)
            .Returns(_ => throw new AutoMapperMappingException("Result mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Result mapping error");

        await _messageBusService.Received(1)
            .PublishAsync(Arg.Any<ProductCreatedEvent>(), Arg.Any<CancellationToken>());
    }
}