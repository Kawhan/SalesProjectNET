using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Sales.CreateSale;
using SalesProject.Application.Sales.Events;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Sales.TestData;

namespace SalesProject.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;
    private readonly CreateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _branchRepository = Substitute.For<IBranchRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _messageBusService = Substitute.For<IMessageBusService>();

        _handler = new CreateSaleHandler(
            _saleRepository,
            _userRepository,
            _branchRepository,
            _productRepository,
            _mapper,
            _messageBusService);
    }

    /// <summary>
    /// Tests that a valid sale creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
        {
            new()
            {
                ProductId = productId,
                Quantity = 1
            }
        }
        };

        var user = CreateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = CreateSaleHandlerTestData.GenerateBranch(command.BranchId);
        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        var sale = CreateSaleHandlerTestData.GenerateSaleFromCommand(command);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<Sale>(command)
            .Returns(sale);

        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo =>
            {
                var createdSale = callInfo.Arg<Sale>();
                return CreateSaleHandlerTestData.GenerateSaleResult(createdSale);
            });

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.UserId.Should().Be(command.UserId);
        result.BranchId.Should().Be(command.BranchId);
        result.Items.Should().HaveCount(1);
        result.TotalAmount.Should().Be(100);

        await _saleRepository.Received(1)
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When creating sale Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new CreateSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _userRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _branchRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the user does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing user When creating sale Then throws key not found exception")]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with id {command.UserId} was not found.");

        await _branchRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the branch does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing branch When creating sale Then throws key not found exception")]
    public async Task Handle_BranchNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var user = CreateSaleHandlerTestData.GenerateUser(command.UserId);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns((Branch?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Branch with id {command.BranchId} was not found.");

        await _productRepository.DidNotReceive()
            .GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when one or more products do not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing product When creating sale Then throws key not found exception")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        var user = CreateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = CreateSaleHandlerTestData.GenerateBranch(command.BranchId);
        var sale = CreateSaleHandlerTestData.GenerateSaleFromCommand(command);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<Sale>(command)
            .Returns(sale);

        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product>());

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("One or more products were not found.");

        await _saleRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an InvalidOperationException is thrown when one or more products are inactive.
    /// </summary>
    [Fact(DisplayName = "Given inactive product When creating sale Then throws invalid operation exception")]
    public async Task Handle_InactiveProduct_ThrowsInvalidOperationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;

        var user = CreateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = CreateSaleHandlerTestData.GenerateBranch(command.BranchId);
        var sale = CreateSaleHandlerTestData.GenerateSaleFromCommand(command);
        var inactiveProduct = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100, status: ProductStatus.Inactive);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<Sale>(command)
            .Returns(sale);

        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { inactiveProduct });

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("One or more products are inactive.");

        await _saleRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that repeated items with the same product are merged before creating the sale.
    /// </summary>
    [Fact(DisplayName = "Given repeated sale items When creating sale Then merges repeated items")]
    public async Task Handle_ValidRequest_MergesRepeatedItems()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 },
                new() { ProductId = productId, Quantity = 3 }
            }
        };

        var user = CreateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = CreateSaleHandlerTestData.GenerateBranch(command.BranchId);
        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        var sale = CreateSaleHandlerTestData.GenerateSaleFromCommand(command);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<Sale>(command)
            .Returns(sale);

        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => CreateSaleHandlerTestData.GenerateSaleResult(callInfo.Arg<Sale>()));

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.Items.Count == 1 &&
                s.Items[0].ProductId == productId &&
                s.Items[0].Quantity == 5),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that no discount is applied when quantity is lower than 4.
    /// </summary>
    [Fact(DisplayName = "Given item quantity lower than four When creating sale Then applies no discount")]
    public async Task Handle_ValidRequest_AppliesNoDiscount()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 3 }
            }
        };

        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.Items[0].DiscountPercentage == 0 &&
                s.Items[0].Discount == 0 &&
                s.Items[0].TotalAmount == 300 &&
                s.TotalAmount == 300),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that 10 percent discount is applied when quantity is between 4 and 9.
    /// </summary>
    [Fact(DisplayName = "Given item quantity between four and nine When creating sale Then applies ten percent discount")]
    public async Task Handle_ValidRequest_AppliesTenPercentDiscount()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 4 }
            }
        };

        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.Items[0].DiscountPercentage == 10 &&
                s.Items[0].Discount == 40 &&
                s.Items[0].TotalAmount == 360 &&
                s.TotalAmount == 360),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that 20 percent discount is applied when quantity is between 10 and 20.
    /// </summary>
    [Fact(DisplayName = "Given item quantity between ten and twenty When creating sale Then applies twenty percent discount")]
    public async Task Handle_ValidRequest_AppliesTwentyPercentDiscount()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 10 }
            }
        };

        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.Items[0].DiscountPercentage == 20 &&
                s.Items[0].Discount == 200 &&
                s.Items[0].TotalAmount == 800 &&
                s.TotalAmount == 800),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the sale total is calculated using all item totals.
    /// </summary>
    [Fact(DisplayName = "Given multiple sale items When creating sale Then calculates sale total")]
    public async Task Handle_ValidRequest_CalculatesSaleTotal()
    {
        // Given
        var firstProductId = Guid.NewGuid();
        var secondProductId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = firstProductId, Quantity = 3 },
                new() { ProductId = secondProductId, Quantity = 4 }
            }
        };

        var firstProduct = CreateSaleHandlerTestData.GenerateProduct(firstProductId, price: 100);
        var secondProduct = CreateSaleHandlerTestData.GenerateProduct(secondProductId, price: 50);

        SetupValidSaleFlow(command, new List<Product> { firstProduct, secondProduct });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.TotalAmount == 480 &&
                s.Items.Count == 2),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that sale items are marked as active after calculation.
    /// </summary>
    [Fact(DisplayName = "Given valid sale items When creating sale Then marks items as active")]
    public async Task Handle_ValidRequest_MarksItemsAsActive()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;

        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.Items.All(item => item.Status == SaleItemStatus.Active)),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that only distinct product ids are sent to the product repository.
    /// </summary>
    [Fact(DisplayName = "Given repeated products When creating sale Then gets distinct products")]
    public async Task Handle_ValidRequest_GetsDistinctProducts()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 },
                new() { ProductId = productId, Quantity = 3 }
            }
        };

        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).GetByIdsAsync(
            Arg.Is<IEnumerable<Guid>>(ids =>
                ids.Count() == 1 &&
                ids.Contains(productId)),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a SaleCreatedEvent is published after creating the sale.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When sale is created Then publishes sale created event")]
    public async Task Handle_ValidRequest_PublishesSaleCreatedEvent()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
        {
            new()
            {
                ProductId = productId,
                Quantity = 1
            }
        }
        };

        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<SaleCreatedEvent>(e =>
                e.UserId == command.UserId &&
                e.BranchId == command.BranchId &&
                e.TotalAmount == 100),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when sale repository fails.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When sale repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenSaleRepositoryFails_ThrowsException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;
        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns<Task<Sale>>(_ => throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when message bus fails.
    /// </summary>
    [Fact(DisplayName = "Given created sale When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;
        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        _messageBusService
            .PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _saleRepository.Received(1)
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<CreateSaleResult>(Arg.Any<Sale>());
    }

    /// <summary>
    /// Tests that an exception is thrown when mapper fails to map command to sale.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When sale mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenSaleMapperFails_ThrowsException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        var user = CreateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = CreateSaleHandlerTestData.GenerateBranch(command.BranchId);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<Sale>(command)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _productRepository.DidNotReceive()
            .GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when result mapper fails.
    /// </summary>
    [Fact(DisplayName = "Given created sale When result mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenResultMapperFails_ThrowsException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;
        var product = CreateSaleHandlerTestData.GenerateProduct(productId, price: 100);

        SetupValidSaleFlow(command, new List<Product> { product });

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(_ => throw new AutoMapperMappingException("Result mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Result mapping error");

        await _messageBusService.Received(1)
            .PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Configures the dependencies for a valid sale creation flow.
    /// </summary>
    private void SetupValidSaleFlow(CreateSaleCommand command, List<Product> products)
    {
        var user = CreateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = CreateSaleHandlerTestData.GenerateBranch(command.BranchId);
        var sale = CreateSaleHandlerTestData.GenerateSaleFromCommand(command);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<Sale>(command)
            .Returns(sale);

        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(products);

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo =>
            {
                var createdSale = callInfo.Arg<Sale>();
                return CreateSaleHandlerTestData.GenerateSaleResult(createdSale);
            });
    }
}