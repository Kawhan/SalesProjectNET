using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Sales.Events;
using SalesProject.Application.Sales.ReactivateSale;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Sales.TestData;

namespace SalesProject.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="ReactivateSaleHandler"/> class.
/// </summary>
public class ReactivateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMessageBusService _messageBusService;
    private readonly ReactivateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public ReactivateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new ReactivateSaleHandler(_saleRepository, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid sale reactivation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When reactivating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        var sale = ReactivateSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;
        sale.Status = SaleStatus.Active;

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        await _saleRepository.Received(1)
            .ReactivateAsync(command.Id, Arg.Any<CancellationToken>());

        await _saleRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale reactivation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale id When reactivating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new ReactivateSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale id When reactivating sale Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new ReactivateSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _saleRepository.DidNotReceive()
            .ReactivateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleReactivatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the sale does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing sale id When reactivating sale Then throws key not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found.");

        await _saleRepository.Received(1)
            .ReactivateAsync(command.Id, Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleReactivatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the repository is called with the correct sale id.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When reactivating sale Then calls repository with correct id")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        var sale = ReactivateSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).ReactivateAsync(
            Arg.Is<Guid>(id => id == command.Id),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the reactivated sale is retrieved after successful reactivation.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When reactivating sale Then gets reactivated sale data")]
    public async Task Handle_ValidRequest_GetsReactivatedSaleData()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        var sale = ReactivateSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1)
            .GetByIdAsync(
                Arg.Is<Guid>(id => id == command.Id),
                Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a SaleReactivatedEvent is published after reactivating the sale.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When sale is reactivated Then publishes sale reactivated event")]
    public async Task Handle_ValidRequest_PublishesSaleReactivatedEvent()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        var sale = ReactivateSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<SaleReactivatedEvent>(e =>
                e.SaleId == sale.Id &&
                e.SaleNumber == sale.SaleNumber &&
                e.UserId == sale.UserId &&
                e.BranchId == sale.BranchId &&
                e.TotalAmount == sale.TotalAmount &&
                e.CreatedAt == sale.CreatedAt &&
                e.UpdatedAt == sale.UpdatedAt.GetValueOrDefault(DateTime.UtcNow)),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the reactivated sale cannot be retrieved.
    /// </summary>
    [Fact(DisplayName = "Given reactivated sale When get by id returns null Then throws key not found exception")]
    public async Task Handle_ValidRequest_WhenGetByIdReturnsNull_ThrowsKeyNotFoundException()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found.");

        await _saleRepository.Received(1)
            .ReactivateAsync(command.Id, Arg.Any<CancellationToken>());

        await _saleRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleReactivatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when ReactivateAsync fails.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When reactivate repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenReactivateRepositoryFails_ThrowsException()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<bool>>(_ => throw new InvalidOperationException("Repository reactivate error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository reactivate error");

        await _saleRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleReactivatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when GetByIdAsync fails after reactivation.
    /// </summary>
    [Fact(DisplayName = "Given reactivated sale When get by id fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenGetByIdFails_ThrowsException()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<Sale?>>(_ => throw new InvalidOperationException("Repository get error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository get error");

        await _saleRepository.Received(1)
            .ReactivateAsync(command.Id, Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleReactivatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails to publish the reactivated event.
    /// </summary>
    [Fact(DisplayName = "Given reactivated sale When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = ReactivateSaleHandlerTestData.GenerateValidCommand();

        var sale = ReactivateSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.ReactivateAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _messageBusService
            .PublishAsync(Arg.Any<SaleReactivatedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _saleRepository.Received(1)
            .ReactivateAsync(command.Id, Arg.Any<CancellationToken>());

        await _saleRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
}