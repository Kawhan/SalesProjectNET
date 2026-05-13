using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Branches.DeleteBranch;
using SalesProject.Application.Branches.Events;
using SalesProject.Application.Common.Messaging;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Branches.TestData;

namespace SalesProject.Unit.Application.Branches;

/// <summary>
/// Contains unit tests for the <see cref="DeleteBranchHandler"/> class.
/// </summary>
public class DeleteBranchHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMessageBusService _messageBusService;
    private readonly DeleteBranchHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteBranchHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public DeleteBranchHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new DeleteBranchHandler(_branchRepository, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid branch delete request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When deleting branch Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = DeleteBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = command.Id,
            Name = "Main Branch",
            Address = "Test Address",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _branchRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _branchRepository.GetByIdAsync(command.Id)
            .Returns(branch);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        await _branchRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid branch delete request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch id When deleting branch Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new DeleteBranchCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch id When deleting branch Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new DeleteBranchCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _branchRepository.DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _branchRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the branch does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing branch id When deleting branch Then throws key not found exception")]
    public async Task Handle_BranchNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = DeleteBranchHandlerTestData.GenerateValidCommand();

        _branchRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Branch with ID {command.Id} not found.");

        await _branchRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _branchRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the delete repository method is called with the correct branch id.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When deleting branch Then calls repository with correct id")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = DeleteBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = command.Id,
            Name = "Main Branch",
            Address = "Test Address",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _branchRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _branchRepository.GetByIdAsync(command.Id)
            .Returns(branch);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _branchRepository.Received(1)
            .DeleteAsync(
                Arg.Is<Guid>(id => id == command.Id),
                Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the deleted branch is retrieved after successful delete.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When deleting branch Then gets deleted branch data")]
    public async Task Handle_ValidRequest_GetsDeletedBranchData()
    {
        // Given
        var command = DeleteBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = command.Id,
            Name = "Main Branch",
            Address = "Test Address",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _branchRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _branchRepository.GetByIdAsync(command.Id)
            .Returns(branch);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _branchRepository.Received(1)
            .GetByIdAsync(Arg.Is<Guid>(id => id == command.Id));
    }

    /// <summary>
    /// Tests that a BranchDeletedEvent is published after deleting the branch.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When branch is deleted Then publishes branch deleted event")]
    public async Task Handle_ValidRequest_PublishesBranchDeletedEvent()
    {
        // Given
        var command = DeleteBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = command.Id,
            Name = "Main Branch",
            Address = "Test Address",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _branchRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _branchRepository.GetByIdAsync(command.Id)
            .Returns(branch);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<BranchDeletedEvent>(e =>
                e.Id == branch.Id &&
                e.Name == branch.Name &&
                e.Address == branch.Address &&
                e.Status == branch.Status &&
                e.CreatedAt == branch.CreatedAt &&
                e.UpdatedAt == branch.UpdatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to delete the branch.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When repository delete fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenDeleteRepositoryFails_ThrowsException()
    {
        // Given
        var command = DeleteBranchHandlerTestData.GenerateValidCommand();

        _branchRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<bool>>(_ => throw new InvalidOperationException("Repository delete error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository delete error");

        await _branchRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get the deleted branch.
    /// </summary>
    [Fact(DisplayName = "Given deleted branch When get by id fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenGetByIdFails_ThrowsException()
    {
        // Given
        var command = DeleteBranchHandlerTestData.GenerateValidCommand();

        _branchRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _branchRepository.GetByIdAsync(command.Id)
            .Returns<Task<Branch>>(_ => throw new InvalidOperationException("Repository get error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository get error");

        await _branchRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails to publish the deleted event.
    /// </summary>
    [Fact(DisplayName = "Given deleted branch When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = DeleteBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = command.Id,
            Name = "Main Branch",
            Address = "Test Address",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _branchRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _branchRepository.GetByIdAsync(command.Id)
            .Returns(branch);

        _messageBusService
            .PublishAsync(Arg.Any<BranchDeletedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _branchRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());

        await _branchRepository.Received(1)
            .GetByIdAsync(command.Id);
    }
}