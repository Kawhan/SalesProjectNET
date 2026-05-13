using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Branches.GetBranch;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Branches.TestData;

namespace SalesProject.Unit.Application.Branches;

/// <summary>
/// Contains unit tests for the <see cref="GetBranchHandler"/> class.
/// </summary>
public class GetBranchHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly GetBranchHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBranchHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetBranchHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetBranchHandler(_branchRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get branch request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When getting branch Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = GetBranchHandlerTestData.GenerateValidCommand();

        var branch = GetBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        var result = GetBranchHandlerTestData.GenerateBranchResult(branch);

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<GetBranchResult>(branch)
            .Returns(result);

        // When
        var getBranchResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getBranchResult.Should().NotBeNull();
        getBranchResult.Id.Should().Be(branch.Id);
        getBranchResult.Name.Should().Be(branch.Name);
        getBranchResult.Address.Should().Be(branch.Address);
        getBranchResult.Status.Should().Be(branch.Status);
        getBranchResult.CreatedAt.Should().Be(branch.CreatedAt);
        getBranchResult.UpdatedAt.Should().Be(branch.UpdatedAt);
    }

    /// <summary>
    /// Tests that an invalid get branch request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch id When getting branch Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new GetBranchCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch id When getting branch Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new GetBranchCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _branchRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<GetBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the branch does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing branch id When getting branch Then throws key not found exception")]
    public async Task Handle_BranchNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = GetBranchHandlerTestData.GenerateValidCommand();

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

        _mapper.DidNotReceive()
            .Map<GetBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that the repository is called with the correct branch id.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When getting branch Then calls repository with correct id")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = GetBranchHandlerTestData.GenerateValidCommand();

        var branch = GetBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        var result = GetBranchHandlerTestData.GenerateBranchResult(branch);

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<GetBranchResult>(branch)
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
    /// Tests that the mapper is called with the branch returned by repository.
    /// </summary>
    [Fact(DisplayName = "Given existing branch When getting branch Then maps branch to result")]
    public async Task Handle_ValidRequest_MapsBranchToResult()
    {
        // Given
        var command = GetBranchHandlerTestData.GenerateValidCommand();

        var branch = GetBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        var result = GetBranchHandlerTestData.GenerateBranchResult(branch);

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<GetBranchResult>(branch)
            .Returns(result);

        // When
        var getBranchResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1)
            .Map<GetBranchResult>(Arg.Is<Branch>(b =>
                b.Id == branch.Id &&
                b.Name == branch.Name &&
                b.Address == branch.Address &&
                b.Status == branch.Status &&
                b.CreatedAt == branch.CreatedAt &&
                b.UpdatedAt == branch.UpdatedAt));

        getBranchResult.Should().BeEquivalentTo(result);
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get the branch.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenRepositoryFails_ThrowsException()
    {
        // Given
        var command = GetBranchHandlerTestData.GenerateValidCommand();

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<Branch?>>(_ => throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        _mapper.DidNotReceive()
            .Map<GetBranchResult>(Arg.Any<Branch>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the mapper fails to map the branch.
    /// </summary>
    [Fact(DisplayName = "Given existing branch When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = GetBranchHandlerTestData.GenerateValidCommand();

        var branch = GetBranchHandlerTestData.GenerateBranch();
        branch.Id = command.Id;

        _branchRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(branch);

        _mapper.Map<GetBranchResult>(branch)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _branchRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
}