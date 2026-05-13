using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Products.Events;
using SalesProject.Application.Products.UpdateProduct;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Products.TestData;

namespace SalesProject.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="UpdateProductHandler"/> class.
/// </summary>
public class UpdateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;
    private readonly UpdateProductHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProductHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public UpdateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new UpdateProductHandler(_productRepository, _mapper, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid product update request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When updating product Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        var updatedProduct = existingProduct;

        var result = new UpdateProductResult
        {
            Id = command.Id,
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status
        };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(existingProduct)
            .Returns(updatedProduct);

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(updatedProduct);

        _mapper.Map<UpdateProductResult>(updatedProduct)
            .Returns(result);

        // When
        var updateProductResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateProductResult.Should().NotBeNull();
        updateProductResult.Id.Should().Be(command.Id);
        updateProductResult.Name.Should().Be(command.Name);
        updateProductResult.CurrentPrice.Should().Be(command.CurrentPrice);
        updateProductResult.Status.Should().Be(command.Status);

        await _productRepository.Received(1)
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid product update request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid product data When updating product Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new UpdateProductCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid product data When updating product Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new UpdateProductCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _productRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<Product>(Arg.Any<Product>());

        _mapper.DidNotReceive()
            .Map<UpdateProductResult>(Arg.Any<Product>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the product does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing product id When updating product Then throws key not found exception")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Product with ID {command.Id} was not exists");

        await _productRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an InvalidOperationException is thrown when another product has the same name.
    /// </summary>
    [Fact(DisplayName = "Given duplicate product name When updating product Then throws invalid operation exception")]
    public async Task Handle_DuplicateProductName_ThrowsInvalidOperationException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        var duplicateProduct = UpdateProductHandlerTestData.GenerateProduct();
        duplicateProduct.Id = Guid.NewGuid();
        duplicateProduct.Name = command.Name;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns(duplicateProduct);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"Product with name {command.Name} already exists");

        await _productRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _productRepository.Received(1)
            .GetByNameAsync(command.Name, Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that updating with the same product name is allowed when the found product has the same id.
    /// </summary>
    [Fact(DisplayName = "Given same product name and same id When updating product Then allows update")]
    public async Task Handle_SameProductNameAndSameId_AllowsUpdate()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;
        existingProduct.Name = command.Name;

        var productWithSameName = UpdateProductHandlerTestData.GenerateProduct();
        productWithSameName.Id = command.Id;
        productWithSameName.Name = command.Name;

        var result = new UpdateProductResult
        {
            Id = command.Id,
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status
        };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns(productWithSameName);

        _mapper.Map<Product>(existingProduct)
            .Returns(existingProduct);

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _mapper.Map<UpdateProductResult>(existingProduct)
            .Returns(result);

        // When
        var updateProductResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateProductResult.Should().NotBeNull();
        updateProductResult.Id.Should().Be(command.Id);

        await _productRepository.Received(1)
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the repository gets the product by the correct id.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When updating product Then gets product by correct id")]
    public async Task Handle_ValidRequest_GetsProductByCorrectId()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        var result = UpdateProductHandlerTestData.GenerateProductResult(existingProduct);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(existingProduct)
            .Returns(existingProduct);

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _mapper.Map<UpdateProductResult>(existingProduct)
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
    /// Tests that the repository checks if another product with the same name exists.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When updating product Then checks product by name")]
    public async Task Handle_ValidRequest_ChecksProductByName()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        var result = UpdateProductHandlerTestData.GenerateProductResult(existingProduct);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(existingProduct)
            .Returns(existingProduct);

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _mapper.Map<UpdateProductResult>(existingProduct)
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
    /// Tests that the product is updated with command data before saving.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When updating product Then updates product with command data")]
    public async Task Handle_ValidRequest_UpdatesProductWithCommandData()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(Arg.Any<Product>())
            .Returns(callInfo => callInfo.Arg<Product>());

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Product>());

        _mapper.Map<UpdateProductResult>(Arg.Any<Product>())
            .Returns(callInfo =>
            {
                var product = callInfo.Arg<Product>();

                return new UpdateProductResult
                {
                    Id = product.Id,
                    Name = product.Name,
                    CurrentPrice = product.CurrentPrice,
                    Status = product.Status
                };
            });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).UpdateAsync(
            Arg.Is<Product>(p =>
                p.Id == command.Id &&
                p.Name == command.Name &&
                p.CurrentPrice == command.CurrentPrice &&
                p.Status == command.Status &&
                p.UpdatedAt.HasValue),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a ProductModifiedEvent is published after updating the product.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When product is updated Then publishes product modified event")]
    public async Task Handle_ValidRequest_PublishesProductModifiedEvent()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(Arg.Any<Product>())
            .Returns(callInfo => callInfo.Arg<Product>());

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Product>());

        _mapper.Map<UpdateProductResult>(Arg.Any<Product>())
            .Returns(callInfo =>
            {
                var product = callInfo.Arg<Product>();

                return new UpdateProductResult
                {
                    Id = product.Id,
                    Name = product.Name,
                    CurrentPrice = product.CurrentPrice,
                    Status = product.Status
                };
            });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<ProductModifiedEvent>(e =>
                e.Id == command.Id &&
                e.Name == command.Name &&
                e.CurrentPrice == command.CurrentPrice &&
                e.Status == command.Status &&
                e.CreatedAt == existingProduct.CreatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the updated product is mapped to result.
    /// </summary>
    [Fact(DisplayName = "Given updated product When handling Then maps updated product to result")]
    public async Task Handle_ValidRequest_MapsUpdatedProductToResult()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        var result = new UpdateProductResult
        {
            Id = command.Id,
            Name = command.Name,
            CurrentPrice = command.CurrentPrice,
            Status = command.Status
        };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(Arg.Any<Product>())
            .Returns(callInfo => callInfo.Arg<Product>());

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Product>());

        _mapper.Map<UpdateProductResult>(Arg.Any<Product>())
            .Returns(result);

        // When
        var updateProductResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1)
            .Map<UpdateProductResult>(Arg.Is<Product>(p =>
                p.Id == command.Id &&
                p.Name == command.Name &&
                p.CurrentPrice == command.CurrentPrice &&
                p.Status == command.Status));

        updateProductResult.Should().BeEquivalentTo(result);
    }

    /// <summary>
    /// Tests that an exception is thrown when GetByIdAsync fails.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When get by id repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenGetByIdRepositoryFails_ThrowsException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<Product?>>(_ => throw new InvalidOperationException("Repository get by id error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository get by id error");

        await _productRepository.DidNotReceive()
            .GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when GetByNameAsync fails.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When get by name repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenGetByNameRepositoryFails_ThrowsException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns<Task<Product?>>(_ => throw new InvalidOperationException("Repository get by name error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository get by name error");

        await _productRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the mapper fails to map product to product.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When product mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenProductMapperFails_ThrowsException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(existingProduct)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _productRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when UpdateAsync fails.
    /// </summary>
    [Fact(DisplayName = "Given valid product data When update repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenUpdateRepositoryFails_ThrowsException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(existingProduct)
            .Returns(existingProduct);

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns<Task<Product>>(_ => throw new InvalidOperationException("Repository update error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository update error");

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateProductResult>(Arg.Any<Product>());
    }

    /// <summary>
    /// Tests that an exception is thrown when message bus fails.
    /// </summary>
    [Fact(DisplayName = "Given updated product When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(existingProduct)
            .Returns(existingProduct);

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _messageBusService
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _productRepository.Received(1)
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateProductResult>(Arg.Any<Product>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the result mapper fails.
    /// </summary>
    [Fact(DisplayName = "Given updated product When result mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenResultMapperFails_ThrowsException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        var existingProduct = UpdateProductHandlerTestData.GenerateProduct();
        existingProduct.Id = command.Id;

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        _mapper.Map<Product>(existingProduct)
            .Returns(existingProduct);

        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        _mapper.Map<UpdateProductResult>(existingProduct)
            .Returns(_ => throw new AutoMapperMappingException("Result mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Result mapping error");

        await _messageBusService.Received(1)
            .PublishAsync(Arg.Any<ProductModifiedEvent>(), Arg.Any<CancellationToken>());
    }
}