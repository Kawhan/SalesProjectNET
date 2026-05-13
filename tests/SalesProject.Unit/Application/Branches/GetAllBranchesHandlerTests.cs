using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Branches.GetAllBranches;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Branches.TestData;

namespace SalesProject.Unit.Application.Branches;

/// <summary>
/// Contains unit tests for the <see cref="GetAllBranchesHandler"/> class.
/// </summary>
public class GetAllBranchesHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly GetAllBranchesHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllBranchesHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetAllBranchesHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetAllBranchesHandler(_branchRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get all branches request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting branches Then returns paginated response")]
    public async Task Handle_ValidRequest_ReturnsPaginatedResponse()
    {
        // Given
        var command = GetAllBranchesHandlerTestData.GenerateValidCommand();
        command.PageNumber = 2;
        command.PageSize = 10;

        var branches = GetAllBranchesHandlerTestData.GenerateBranches(10);
        var branchResults = GetAllBranchesHandlerTestData.GenerateBranchResults(branches);

        _branchRepository.GetAllAsync(
                command.PageNumber,
                command.PageSize,
                command.Name,
                command.Address,
                command.Status,
                Arg.Any<CancellationToken>())
            .Returns((Branches: branches, TotalCount: 25));

        _mapper.Map<List<GetAllBranchesResult>>(branches)
            .Returns(branchResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(10);
        result.Data.Should().BeEquivalentTo(branchResults);
        result.CurrentPage.Should().Be(command.PageNumber);
        result.PageSize.Should().Be(command.PageSize);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
    }

    /// <summary>
    /// Tests that the repository is called with the correct pagination and filter parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting branches Then calls repository with correct filters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectFilters()
    {
        // Given
        var command = GetAllBranchesHandlerTestData.GenerateValidCommand();

        var branches = GetAllBranchesHandlerTestData.GenerateBranches(3);
        var branchResults = GetAllBranchesHandlerTestData.GenerateBranchResults(branches);

        _branchRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<BranchStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Branches: branches, TotalCount: branches.Count));

        _mapper.Map<List<GetAllBranchesResult>>(branches)
            .Returns(branchResults);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _branchRepository.Received(1).GetAllAsync(
            Arg.Is<int>(pageNumber => pageNumber == command.PageNumber),
            Arg.Is<int>(pageSize => pageSize == command.PageSize),
            Arg.Is<string?>(name => name == command.Name),
            Arg.Is<string?>(address => address == command.Address),
            Arg.Is<BranchStatus?>(status => status == command.Status),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that branches returned by the repository are mapped to result objects.
    /// </summary>
    [Fact(DisplayName = "Given branches from repository When getting branches Then maps branches to result")]
    public async Task Handle_ValidRequest_MapsBranchesToResult()
    {
        // Given
        var command = GetAllBranchesHandlerTestData.GenerateValidCommand();

        var branches = GetAllBranchesHandlerTestData.GenerateBranches(5);
        var branchResults = GetAllBranchesHandlerTestData.GenerateBranchResults(branches);

        _branchRepository.GetAllAsync(
                command.PageNumber,
                command.PageSize,
                command.Name,
                command.Address,
                command.Status,
                Arg.Any<CancellationToken>())
            .Returns((Branches: branches, TotalCount: branches.Count));

        _mapper.Map<List<GetAllBranchesResult>>(branches)
            .Returns(branchResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<List<GetAllBranchesResult>>(
            Arg.Is<List<Branch>>(b => b == branches));

        result.Data.Should().BeEquivalentTo(branchResults);
    }

    /// <summary>
    /// Tests that total pages are calculated correctly when total count is divisible by page size.
    /// </summary>
    [Fact(DisplayName = "Given total count divisible by page size When getting branches Then calculates total pages correctly")]
    public async Task Handle_ValidRequest_WhenTotalCountIsDivisibleByPageSize_CalculatesTotalPages()
    {
        // Given
        var command = GetAllBranchesHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var branches = GetAllBranchesHandlerTestData.GenerateBranches(10);
        var branchResults = GetAllBranchesHandlerTestData.GenerateBranchResults(branches);

        _branchRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<BranchStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Branches: branches, TotalCount: 30));

        _mapper.Map<List<GetAllBranchesResult>>(branches)
            .Returns(branchResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(3);
        result.TotalCount.Should().Be(30);
    }

    /// <summary>
    /// Tests that total pages are rounded up when total count is not divisible by page size.
    /// </summary>
    [Fact(DisplayName = "Given total count not divisible by page size When getting branches Then rounds total pages up")]
    public async Task Handle_ValidRequest_WhenTotalCountIsNotDivisibleByPageSize_RoundsTotalPagesUp()
    {
        // Given
        var command = GetAllBranchesHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var branches = GetAllBranchesHandlerTestData.GenerateBranches(10);
        var branchResults = GetAllBranchesHandlerTestData.GenerateBranchResults(branches);

        _branchRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<BranchStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Branches: branches, TotalCount: 31));

        _mapper.Map<List<GetAllBranchesResult>>(branches)
            .Returns(branchResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(4);
        result.TotalCount.Should().Be(31);
    }

    /// <summary>
    /// Tests that an empty repository result returns an empty paginated response.
    /// </summary>
    [Fact(DisplayName = "Given no branches When getting branches Then returns empty paginated response")]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResponse()
    {
        // Given
        var command = GetAllBranchesHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var branches = new List<Branch>();
        var branchResults = new List<GetAllBranchesResult>();

        _branchRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<BranchStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Branches: branches, TotalCount: 0));

        _mapper.Map<List<GetAllBranchesResult>>(branches)
            .Returns(branchResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.CurrentPage.Should().Be(command.PageNumber);
        result.PageSize.Should().Be(command.PageSize);
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    /// <summary>
    /// Tests that a request without filters calls repository with null filters.
    /// </summary>
    [Fact(DisplayName = "Given request without filters When getting branches Then calls repository with null filters")]
    public async Task Handle_RequestWithoutFilters_CallsRepositoryWithNullFilters()
    {
        // Given
        var command = new GetAllBranchesCommand
        {
            PageNumber = 1,
            PageSize = 10,
            Name = null,
            Address = null,
            Status = null
        };

        var branches = GetAllBranchesHandlerTestData.GenerateBranches(2);
        var branchResults = GetAllBranchesHandlerTestData.GenerateBranchResults(branches);

        _branchRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<BranchStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Branches: branches, TotalCount: branches.Count));

        _mapper.Map<List<GetAllBranchesResult>>(branches)
            .Returns(branchResults);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _branchRepository.Received(1).GetAllAsync(
            Arg.Is<int>(pageNumber => pageNumber == command.PageNumber),
            Arg.Is<int>(pageSize => pageSize == command.PageSize),
            Arg.Is<string?>(name => name == null),
            Arg.Is<string?>(address => address == null),
            Arg.Is<BranchStatus?>(status => status == null),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get branches.
    /// </summary>
    [Fact(DisplayName = "Given valid request When repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenRepositoryFails_ThrowsException()
    {
        // Given
        var command = GetAllBranchesHandlerTestData.GenerateValidCommand();

        _branchRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<BranchStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<(List<Branch> Branches, int TotalCount)>>(_ =>
                throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        _mapper.DidNotReceive()
            .Map<List<GetAllBranchesResult>>(Arg.Any<List<Branch>>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the mapper fails to map branches.
    /// </summary>
    [Fact(DisplayName = "Given branches from repository When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = GetAllBranchesHandlerTestData.GenerateValidCommand();

        var branches = GetAllBranchesHandlerTestData.GenerateBranches(5);

        _branchRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<BranchStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Branches: branches, TotalCount: branches.Count));

        _mapper.Map<List<GetAllBranchesResult>>(branches)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _branchRepository.Received(1).GetAllAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<BranchStatus?>(),
            Arg.Any<CancellationToken>());
    }
}