using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Sales.DeleteSale;
using SalesProject.Application.Sales.Events;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Sales.TestData;

namespace SalesProject.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="DeleteSaleHandler"/> class.
/// </summary>
public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMessageBusService _messageBusService;
    private readonly DeleteSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new DeleteSaleHandler(_saleRepository, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid sale delete request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When deleting sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        var sale = DeleteSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        await _saleRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _saleRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale delete request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale id When deleting sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new DeleteSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale id When deleting sale Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new DeleteSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _saleRepository.DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the sale does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing sale id When deleting sale Then throws key not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");

        await _saleRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _saleRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the repository is called with the correct sale id.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When deleting sale Then calls repository with correct id")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        var sale = DeleteSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).DeleteAsync(
            Arg.Is<Guid>(id => id == command.Id),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the cancelled sale is retrieved after successful delete.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When deleting sale Then gets cancelled sale data")]
    public async Task Handle_ValidRequest_GetsCancelledSaleData()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        var sale = DeleteSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
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
    /// Tests that a SaleCancelledEvent is published after deleting the sale.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When sale is deleted Then publishes sale cancelled event")]
    public async Task Handle_ValidRequest_PublishesSaleCancelledEvent()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        var sale = DeleteSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<SaleCancelledEvent>(e =>
                e.SaleId == command.Id &&
                e.SaleNumber == sale.SaleNumber &&
                e.UserId == sale.UserId &&
                e.BranchId == sale.BranchId &&
                e.TotalAmount == sale.TotalAmount &&
                e.CreatedAt == sale.CreatedAt &&
                e.CancelledAt != default),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to delete the sale.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When delete repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenDeleteRepositoryFails_ThrowsException()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<bool>>(_ => throw new InvalidOperationException("Repository delete error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository delete error");

        await _saleRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get the cancelled sale.
    /// </summary>
    [Fact(DisplayName = "Given deleted sale When get by id fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenGetByIdFails_ThrowsException()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
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
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the cancelled sale cannot be retrieved.
    /// </summary>
    [Fact(DisplayName = "Given deleted sale When get by id returns null Then throws key not found exception")]
    public async Task Handle_ValidRequest_WhenGetByIdReturnsNull_ThrowsKeyNotFoundException()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");

        await _saleRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _saleRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails to publish the cancelled event.
    /// </summary>
    [Fact(DisplayName = "Given deleted sale When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        var sale = DeleteSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _messageBusService
            .PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _saleRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _saleRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
}