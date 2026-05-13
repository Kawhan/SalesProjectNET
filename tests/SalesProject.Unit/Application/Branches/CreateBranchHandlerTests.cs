using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Branches.CreateBranch;
using SalesProject.Application.Branches.Events;
using SalesProject.Application.Common.Messaging;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Branches.TestData;

namespace SalesProject.Unit.Application.Branches;

/// <summary>
/// Contains unit tests for the <see cref="CreateBranchHandler"/> class.
/// </summary>
public class CreateBranchHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;
    private readonly CreateBranchHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBranchHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateBranchHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _mapper = Substitute.For<IMapper>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new CreateBranchHandler(_branchRepository, _mapper, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid branch creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When creating branch Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        var result = new CreateBranchResult
        {
            Id = branch.Id
        };

        _mapper.Map<Branch>(command).Returns(branch);

        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<CreateBranchResult>(branch).Returns(result);

        // When
        var createBranchResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createBranchResult.Should().NotBeNull();
        createBranchResult.Id.Should().Be(branch.Id);

        await _branchRepository.Received(1)
            .CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid branch creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch data When creating branch Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateBranchCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that the mapper is called with the correct command.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps command to branch entity")]
    public async Task Handle_ValidRequest_MapsCommandToBranch()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _mapper.Map<Branch>(command).Returns(branch);

        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<CreateBranchResult>(branch).Returns(new CreateBranchResult
        {
            Id = branch.Id
        });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<Branch>(Arg.Is<CreateBranchCommand>(c =>
            c.Name == command.Name &&
            c.Address == command.Address &&
            c.Status == command.Status));
    }

    /// <summary>
    /// Tests that the branch repository is called with the correct branch entity.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then creates branch in repository")]
    public async Task Handle_ValidRequest_CreatesBranchInRepository()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _mapper.Map<Branch>(command).Returns(branch);

        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<CreateBranchResult>(branch).Returns(new CreateBranchResult
        {
            Id = branch.Id
        });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _branchRepository.Received(1).CreateAsync(
            Arg.Is<Branch>(b =>
                b.Name == command.Name &&
                b.Address == command.Address &&
                b.Status == command.Status),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a BranchCreatedEvent is published after creating the branch.
    /// </summary>
    [Fact(DisplayName = "Given valid command When branch is created Then publishes branch created event")]
    public async Task Handle_ValidRequest_PublishesBranchCreatedEvent()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _mapper.Map<Branch>(command).Returns(branch);

        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<CreateBranchResult>(branch).Returns(new CreateBranchResult
        {
            Id = branch.Id
        });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<BranchCreatedEvent>(e =>
                e.Id == branch.Id &&
                e.Name == branch.Name &&
                e.Address == branch.Address &&
                e.Status == branch.Status &&
                e.CreatedAt == branch.CreatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the result mapper is called with the created branch.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps created branch to result")]
    public async Task Handle_ValidRequest_MapsCreatedBranchToResult()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        var result = new CreateBranchResult
        {
            Id = branch.Id
        };

        _mapper.Map<Branch>(command).Returns(branch);

        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<CreateBranchResult>(branch).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<CreateBranchResult>(Arg.Is<Branch>(b =>
            b.Id == branch.Id &&
            b.Name == branch.Name &&
            b.Address == branch.Address &&
            b.Status == branch.Status &&
            b.CreatedAt == branch.CreatedAt));
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch data When creating branch Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new CreateBranchCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        _mapper.DidNotReceive().Map<Branch>(Arg.Any<CreateBranchCommand>());

        await _branchRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to create the branch.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenRepositoryFails_ThrowsException()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _mapper.Map<Branch>(command).Returns(branch);

        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns<Task<Branch>>(_ => throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails to publish the event.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _mapper.Map<Branch>(command).Returns(branch);

        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        _messageBusService
            .PublishAsync(Arg.Any<BranchCreatedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        await _branchRepository.Received(1)
            .CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive().Map<CreateBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the command cannot be mapped to a branch entity.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        _mapper.Map<Branch>(command)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _branchRepository.DidNotReceive()
            .CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<BranchCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the created branch cannot be mapped to result.
    /// </summary>
    [Fact(DisplayName = "Given created branch When result mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenResultMapperFails_ThrowsException()
    {
        // Given
        var command = CreateBranchHandlerTestData.GenerateValidCommand();

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Status = command.Status,
            CreatedAt = DateTime.UtcNow
        };

        _mapper.Map<Branch>(command).Returns(branch);

        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<CreateBranchResult>(branch)
            .Returns(_ => throw new AutoMapperMappingException("Result mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Result mapping error");

        await _branchRepository.Received(1)
            .CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());

        await _messageBusService.Received(1)
            .PublishAsync(Arg.Any<BranchCreatedEvent>(), Arg.Any<CancellationToken>());
    }
}