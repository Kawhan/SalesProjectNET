using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Branches.Events;
using SalesProject.Application.Branches.UpdateBranch;
using SalesProject.Application.Common.Messaging;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Branches.TestData;

namespace SalesProject.Unit.Application.Branches;

/// <summary>
/// Contains unit tests for the <see cref="UpdateBranchHandler"/> class.
/// </summary>
public class UpdateBranchHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;
    private readonly UpdateBranchHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBranchHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public UpdateBranchHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _mapper = Substitute.For<IMapper>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new UpdateBranchHandler(_branchRepository, _mapper, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid branch update request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When updating branch Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        var branch = UpdateBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        var updatedBranch = branch;

        var result = new UpdateBranchResult
        {
            Id = updatedBranch.Id,
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = updatedBranch.CreatedAt,
            UpdatedAt = updatedBranch.UpdatedAt
        };

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _branchRepository.UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(updatedBranch);

        _mapper.Map<UpdateBranchResult>(updatedBranch)
            .Returns(result);

        // When
        var updateBranchResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateBranchResult.Should().NotBeNull();
        updateBranchResult.Id.Should().Be(command.Id);
        updateBranchResult.Name.Should().Be(command.Name);
        updateBranchResult.Address.Should().Be(command.Address);
        updateBranchResult.Status.Should().Be(command.Status);

        await _branchRepository.Received(1)
            .UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid branch update request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch data When updating branch Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new UpdateBranchCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch data When updating branch Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new UpdateBranchCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _branchRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _branchRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchModifiedEvent>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the branch does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing branch id When updating branch Then throws key not found exception")]
    public async Task Handle_BranchNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Branch?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Branch with id {command.Id} was not found.");

        await _branchRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _branchRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchModifiedEvent>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that the repository is called with the correct branch id.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When updating branch Then gets branch by correct id")]
    public async Task Handle_ValidRequest_GetsBranchByCorrectId()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        var branch = UpdateBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        var result = UpdateBranchHandlerTestData.GenerateBranchResult(branch);

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _branchRepository.UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<UpdateBranchResult>(branch)
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _branchRepository.Received(1)
            .GetByIdAsync(
                Arg.Is<Guid>(id => id == command.Id),
                Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the branch is updated with command data before saving.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When updating branch Then updates branch with command data")]
    public async Task Handle_ValidRequest_UpdatesBranchWithCommandData()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        var branch = UpdateBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        var result = new UpdateBranchResult
        {
            Id = branch.Id,
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = branch.CreatedAt,
            UpdatedAt = branch.UpdatedAt
        };

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _branchRepository.UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Branch>());

        _mapper.Map<UpdateBranchResult>(Arg.Any<Branch>())
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _branchRepository.Received(1).UpdateAsync(
            Arg.Is<Branch>(b =>
                b.Id == command.Id &&
                b.Name == command.Name &&
                b.Address == command.Address &&
                b.Status == command.Status),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a BranchModifiedEvent is published after updating the branch.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When branch is updated Then publishes branch modified event")]
    public async Task Handle_ValidRequest_PublishesBranchModifiedEvent()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        var branch = UpdateBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _branchRepository.UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Branch>());

        _mapper.Map<UpdateBranchResult>(Arg.Any<Branch>())
            .Returns(callInfo =>
            {
                var updated = callInfo.Arg<Branch>();

                return new UpdateBranchResult
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    Address = updated.Address,
                    Status = updated.Status,
                    CreatedAt = updated.CreatedAt,
                    UpdatedAt = updated.UpdatedAt
                };
            });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<BranchModifiedEvent>(e =>
                e.Id == command.Id &&
                e.Name == command.Name &&
                e.Address == command.Address &&
                e.Status == command.Status &&
                e.CreatedAt == branch.CreatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the updated branch is mapped to result.
    /// </summary>
    [Fact(DisplayName = "Given updated branch When handling Then maps updated branch to result")]
    public async Task Handle_ValidRequest_MapsUpdatedBranchToResult()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        var branch = UpdateBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        var result = new UpdateBranchResult
        {
            Id = command.Id,
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = branch.CreatedAt,
            UpdatedAt = branch.UpdatedAt
        };

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _branchRepository.UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Branch>());

        _mapper.Map<UpdateBranchResult>(Arg.Any<Branch>())
            .Returns(result);

        // When
        var updateBranchResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1)
            .Map<UpdateBranchResult>(Arg.Is<Branch>(b =>
                b.Id == command.Id &&
                b.Name == command.Name &&
                b.Address == command.Address &&
                b.Status == command.Status));

        updateBranchResult.Should().BeEquivalentTo(result);
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get the branch.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When get repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenGetRepositoryFails_ThrowsException()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<Branch?>>(_ => throw new InvalidOperationException("Repository get error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository get error");

        await _branchRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchModifiedEvent>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to update the branch.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When update repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenUpdateRepositoryFails_ThrowsException()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        var branch = UpdateBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _branchRepository.UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns<Task<Branch>>(_ => throw new InvalidOperationException("Repository update error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository update error");

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchModifiedEvent>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails to publish the modified event.
    /// </summary>
    [Fact(DisplayName = "Given updated branch When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        var branch = UpdateBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _branchRepository.UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Branch>());

        _messageBusService
            .PublishAsync(Arg.Any<BranchModifiedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _branchRepository.Received(1)
            .UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<UpdateBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the mapper fails to map the updated branch.
    /// </summary>
    [Fact(DisplayName = "Given updated branch When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = UpdateBranchHandlerTestData.GenerateValidCommand();

        var branch = UpdateBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _branchRepository.UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Branch>());

        _mapper.Map<UpdateBranchResult>(Arg.Any<Branch>())
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _messageBusService.Received(1)
            .PublishAsync(Arg.Any<BranchModifiedEvent>(), Arg.Any<CancellationToken>());
    }
}