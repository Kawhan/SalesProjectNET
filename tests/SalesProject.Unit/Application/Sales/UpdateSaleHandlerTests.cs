using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Sales.Events;
using SalesProject.Application.Sales.UpdateSale;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Sales.TestData;

namespace SalesProject.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;
    private readonly UpdateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _branchRepository = Substitute.For<IBranchRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _messageBusService = Substitute.For<IMessageBusService>();

        _handler = new UpdateSaleHandler(
            _saleRepository,
            _userRepository,
            _branchRepository,
            _productRepository,
            _mapper,
            _messageBusService);
    }

    /// <summary>
    /// Tests that a valid sale update request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new()
                {
                    ProductId = productId,
                    Quantity = 2
                }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        var updateSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateSaleResult.Should().NotBeNull();
        updateSaleResult.Id.Should().Be(command.Id);
        updateSaleResult.UserId.Should().Be(command.UserId);
        updateSaleResult.BranchId.Should().Be(command.BranchId);

        await _saleRepository.Received(1)
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale update request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When updating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new UpdateSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When updating sale Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new UpdateSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _saleRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _userRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _branchRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _productRepository.DidNotReceive()
            .GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the sale does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing sale When updating sale Then throws key not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with id {command.Id} was not found.");

        await _userRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that cancelled sales cannot be updated.
    /// </summary>
    [Fact(DisplayName = "Given cancelled sale When updating sale Then throws invalid operation exception")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Status = SaleStatus.Cancelled;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Cancelled sales cannot be updated.");

        await _userRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the user does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing user When updating sale Then throws key not found exception")]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

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
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the branch does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing branch When updating sale Then throws key not found exception")]
    public async Task Handle_BranchNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        var user = UpdateSaleHandlerTestData.GenerateUser(command.UserId);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

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
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when one or more products do not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing product When updating sale Then throws key not found exception")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        var user = UpdateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = UpdateSaleHandlerTestData.GenerateBranch(command.BranchId);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product>());

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("One or more products were not found.");

        await _saleRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an InvalidOperationException is thrown when one or more products are inactive.
    /// </summary>
    [Fact(DisplayName = "Given inactive product When updating sale Then throws invalid operation exception")]
    public async Task Handle_InactiveProduct_ThrowsInvalidOperationException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        var user = UpdateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = UpdateSaleHandlerTestData.GenerateBranch(command.BranchId);
        var inactiveProduct = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100, status: ProductStatus.Inactive);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { inactiveProduct });

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("One or more products are inactive.");

        await _saleRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that repeated items with the same product are grouped before updating the sale.
    /// </summary>
    [Fact(DisplayName = "Given repeated sale items When updating sale Then groups repeated items")]
    public async Task Handle_ValidRequest_GroupsRepeatedItems()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 },
                new() { ProductId = productId, Quantity = 3 }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s =>
                s.Items.Count(item => item.ProductId == productId && item.Status == SaleItemStatus.Active) == 1 &&
                s.Items.First(item => item.ProductId == productId).Quantity == 5),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an existing sale item is updated with new quantity and product price.
    /// </summary>
    [Fact(DisplayName = "Given existing sale item When updating sale Then updates existing item")]
    public async Task Handle_ValidRequest_UpdatesExistingItem()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 4 }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;
        sale.Items[0].Quantity = 1;
        sale.Items[0].UnitPrice = 50;
        sale.Items[0].TotalAmount = 50;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s =>
                s.Items[0].Quantity == 4 &&
                s.Items[0].UnitPrice == 100 &&
                s.Items[0].DiscountPercentage == 10 &&
                s.Items[0].Discount == 40 &&
                s.Items[0].TotalAmount == 360 &&
                s.TotalAmount == 360),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a new sale item is added when the requested product is not already in the sale.
    /// </summary>
    [Fact(DisplayName = "Given new sale item When updating sale Then adds new item")]
    public async Task Handle_ValidRequest_AddsNewItem()
    {
        // Given
        var existingProductId = Guid.NewGuid();
        var newProductId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = existingProductId, Quantity = 2 },
                new() { ProductId = newProductId, Quantity = 3 }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = existingProductId;

        var existingProduct = UpdateSaleHandlerTestData.GenerateProduct(existingProductId, price: 100);
        var newProduct = UpdateSaleHandlerTestData.GenerateProduct(newProductId, price: 50);

        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { existingProduct, newProduct }, result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s =>
                s.Items.Any(item =>
                    item.ProductId == newProductId &&
                    item.Quantity == 3 &&
                    item.UnitPrice == 50 &&
                    item.TotalAmount == 150 &&
                    item.Status == SaleItemStatus.Active)),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that removed sale items are cancelled.
    /// </summary>
    [Fact(DisplayName = "Given removed sale item When updating sale Then cancels removed item")]
    public async Task Handle_ValidRequest_CancelsRemovedItem()
    {
        // Given
        var keptProductId = Guid.NewGuid();
        var removedProductId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = keptProductId, Quantity = 2 }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items = new List<SaleItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = keptProductId,
                Quantity = 1,
                UnitPrice = 100,
                TotalAmount = 100,
                Status = SaleItemStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = removedProductId,
                Quantity = 3,
                UnitPrice = 50,
                TotalAmount = 150,
                Status = SaleItemStatus.Active
            }
        };

        var product = UpdateSaleHandlerTestData.GenerateProduct(keptProductId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s =>
                s.Items.Any(item =>
                    item.ProductId == removedProductId &&
                    item.Status == SaleItemStatus.Cancelled)),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a SaleItemsCancelledEvent is published when items are removed from the sale.
    /// </summary>
    [Fact(DisplayName = "Given removed sale item When updating sale Then publishes sale items cancelled event")]
    public async Task Handle_ValidRequest_PublishesSaleItemsCancelledEvent()
    {
        // Given
        var keptProductId = Guid.NewGuid();
        var removedProductId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = keptProductId, Quantity = 2 }
            }
        };

        var removedItemId = Guid.NewGuid();

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items = new List<SaleItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = keptProductId,
                Quantity = 1,
                UnitPrice = 100,
                TotalAmount = 100,
                Status = SaleItemStatus.Active
            },
            new()
            {
                Id = removedItemId,
                ProductId = removedProductId,
                Quantity = 3,
                UnitPrice = 50,
                DiscountPercentage = 0,
                Discount = 0,
                TotalAmount = 150,
                Status = SaleItemStatus.Active
            }
        };

        var product = UpdateSaleHandlerTestData.GenerateProduct(keptProductId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<SaleItemsCancelledEvent>(e =>
                e.SaleId == sale.Id &&
                e.SaleNumber == sale.SaleNumber &&
                e.Items.Count == 1 &&
                e.Items[0].SaleItemId == removedItemId &&
                e.Items[0].ProductId == removedProductId),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that SaleItemsCancelledEvent is not published when no item is removed.
    /// </summary>
    [Fact(DisplayName = "Given no removed sale items When updating sale Then does not publish sale items cancelled event")]
    public async Task Handle_ValidRequest_DoesNotPublishSaleItemsCancelledEventWhenNoItemsRemoved()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleItemsCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the sale total is recalculated using only active items.
    /// </summary>
    [Fact(DisplayName = "Given active and cancelled items When updating sale Then recalculates total using active items")]
    public async Task Handle_ValidRequest_RecalculatesTotalUsingActiveItems()
    {
        // Given
        var keptProductId = Guid.NewGuid();
        var removedProductId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = keptProductId, Quantity = 4 }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items = new List<SaleItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = keptProductId,
                Quantity = 1,
                UnitPrice = 100,
                TotalAmount = 100,
                Status = SaleItemStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = removedProductId,
                Quantity = 5,
                UnitPrice = 100,
                TotalAmount = 500,
                Status = SaleItemStatus.Active
            }
        };

        var product = UpdateSaleHandlerTestData.GenerateProduct(keptProductId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s =>
                s.TotalAmount == 360 &&
                s.Items.Any(item => item.ProductId == removedProductId && item.Status == SaleItemStatus.Cancelled)),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a SaleModifiedEvent is published after updating the sale.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When sale is updated Then publishes sale modified event")]
    public async Task Handle_ValidRequest_PublishesSaleModifiedEvent()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<SaleModifiedEvent>(e =>
                e.SaleId == sale.Id &&
                e.SaleNumber == sale.SaleNumber &&
                e.UserId == command.UserId &&
                e.BranchId == command.BranchId &&
                e.TotalAmount == sale.TotalAmount &&
                e.CreatedAt == sale.CreatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the updated sale is mapped to result.
    /// </summary>
    [Fact(DisplayName = "Given updated sale When handling Then maps updated sale to result")]
    public async Task Handle_ValidRequest_MapsUpdatedSaleToResult()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        // When
        var updateSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1)
            .Map<UpdateSaleResult>(Arg.Is<Sale>(s => s.Id == sale.Id));

        updateSaleResult.Should().BeEquivalentTo(result);
    }

    /// <summary>
    /// Tests that an exception is thrown when the sale repository fails to update.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When update repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenUpdateRepositoryFails_ThrowsException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns<Task<Sale>>(_ => throw new InvalidOperationException("Repository update error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository update error");

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateSaleResult>(Arg.Any<Sale>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails to publish the modified event.
    /// </summary>
    [Fact(DisplayName = "Given updated sale When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        _messageBusService
            .PublishAsync(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _saleRepository.Received(1)
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateSaleResult>(Arg.Any<Sale>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the result mapper fails.
    /// </summary>
    [Fact(DisplayName = "Given updated sale When result mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenResultMapperFails_ThrowsException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var productId = command.Items[0].ProductId;

        var sale = UpdateSaleHandlerTestData.GenerateSale(command.Id, command.UserId, command.BranchId);
        sale.Items[0].ProductId = productId;

        var product = UpdateSaleHandlerTestData.GenerateProduct(productId, price: 100);
        var result = UpdateSaleHandlerTestData.GenerateSaleResult(sale);

        SetupValidSaleFlow(command, sale, new List<Product> { product }, result);

        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(_ => throw new AutoMapperMappingException("Result mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Result mapping error");

        await _messageBusService.Received(1)
            .PublishAsync(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Configures the dependencies for a valid sale update flow.
    /// </summary>
    private void SetupValidSaleFlow(
        UpdateSaleCommand command,
        Sale sale,
        List<Product> products,
        UpdateSaleResult result)
    {
        var user = UpdateSaleHandlerTestData.GenerateUser(command.UserId);
        var branch = UpdateSaleHandlerTestData.GenerateBranch(command.BranchId);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(branch);

        _productRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(products);

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(result);
    }
}
