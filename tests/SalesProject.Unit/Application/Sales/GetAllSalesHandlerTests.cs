using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Sales.GetAllSales;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Sales.TestData;

namespace SalesProject.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="GetAllSalesHandler"/> class.
/// </summary>
public class GetAllSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetAllSalesHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllSalesHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetAllSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetAllSalesHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get all sales request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting sales Then returns paginated response")]
    public async Task Handle_ValidRequest_ReturnsPaginatedResponse()
    {
        // Given
        var command = GetAllSalesHandlerTestData.GenerateValidCommand();
        command.PageNumber = 2;
        command.PageSize = 10;

        var sales = GetAllSalesHandlerTestData.GenerateSales(10);
        var saleResults = GetAllSalesHandlerTestData.GenerateSaleResults(sales);

        _saleRepository.GetAllAsync(
                command.PageNumber,
                command.PageSize,
                command.SaleNumber,
                command.UserId,
                command.BranchId,
                command.Status,
                command.StartDate,
                command.EndDate,
                command.MinTotalAmount,
                command.MaxTotalAmount,
                Arg.Any<CancellationToken>())
            .Returns((Sales: sales, TotalCount: 25));

        _mapper.Map<List<GetAllSalesResult>>(sales)
            .Returns(saleResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(10);
        result.Data.Should().BeEquivalentTo(saleResults);
        result.CurrentPage.Should().Be(command.PageNumber);
        result.PageSize.Should().Be(command.PageSize);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
    }

    /// <summary>
    /// Tests that the repository is called with the correct pagination and filter parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting sales Then calls repository with correct filters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectFilters()
    {
        // Given
        var command = GetAllSalesHandlerTestData.GenerateValidCommand();

        var sales = GetAllSalesHandlerTestData.GenerateSales(3);
        var saleResults = GetAllSalesHandlerTestData.GenerateSaleResults(sales);

        _saleRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<Guid?>(),
                Arg.Any<SaleStatus?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<CancellationToken>())
            .Returns((Sales: sales, TotalCount: sales.Count));

        _mapper.Map<List<GetAllSalesResult>>(sales)
            .Returns(saleResults);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetAllAsync(
            Arg.Is<int>(pageNumber => pageNumber == command.PageNumber),
            Arg.Is<int>(pageSize => pageSize == command.PageSize),
            Arg.Is<string?>(saleNumber => saleNumber == command.SaleNumber),
            Arg.Is<Guid?>(userId => userId == command.UserId),
            Arg.Is<Guid?>(branchId => branchId == command.BranchId),
            Arg.Is<SaleStatus?>(status => status == command.Status),
            Arg.Is<DateTime?>(startDate => startDate == command.StartDate),
            Arg.Is<DateTime?>(endDate => endDate == command.EndDate),
            Arg.Is<decimal?>(minTotalAmount => minTotalAmount == command.MinTotalAmount),
            Arg.Is<decimal?>(maxTotalAmount => maxTotalAmount == command.MaxTotalAmount),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that sales returned by the repository are mapped to result objects.
    /// </summary>
    [Fact(DisplayName = "Given sales from repository When getting sales Then maps sales to result")]
    public async Task Handle_ValidRequest_MapsSalesToResult()
    {
        // Given
        var command = GetAllSalesHandlerTestData.GenerateValidCommand();

        var sales = GetAllSalesHandlerTestData.GenerateSales(5);
        var saleResults = GetAllSalesHandlerTestData.GenerateSaleResults(sales);

        _saleRepository.GetAllAsync(
                command.PageNumber,
                command.PageSize,
                command.SaleNumber,
                command.UserId,
                command.BranchId,
                command.Status,
                command.StartDate,
                command.EndDate,
                command.MinTotalAmount,
                command.MaxTotalAmount,
                Arg.Any<CancellationToken>())
            .Returns((Sales: sales, TotalCount: sales.Count));

        _mapper.Map<List<GetAllSalesResult>>(sales)
            .Returns(saleResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<List<GetAllSalesResult>>(
            Arg.Is<List<Sale>>(s => s == sales));

        result.Data.Should().BeEquivalentTo(saleResults);
    }

    /// <summary>
    /// Tests that total pages are calculated correctly when total count is divisible by page size.
    /// </summary>
    [Fact(DisplayName = "Given total count divisible by page size When getting sales Then calculates total pages correctly")]
    public async Task Handle_ValidRequest_WhenTotalCountIsDivisibleByPageSize_CalculatesTotalPages()
    {
        // Given
        var command = GetAllSalesHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var sales = GetAllSalesHandlerTestData.GenerateSales(10);
        var saleResults = GetAllSalesHandlerTestData.GenerateSaleResults(sales);

        _saleRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<Guid?>(),
                Arg.Any<SaleStatus?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<CancellationToken>())
            .Returns((Sales: sales, TotalCount: 30));

        _mapper.Map<List<GetAllSalesResult>>(sales)
            .Returns(saleResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(3);
        result.TotalCount.Should().Be(30);
    }

    /// <summary>
    /// Tests that total pages are rounded up when total count is not divisible by page size.
    /// </summary>
    [Fact(DisplayName = "Given total count not divisible by page size When getting sales Then rounds total pages up")]
    public async Task Handle_ValidRequest_WhenTotalCountIsNotDivisibleByPageSize_RoundsTotalPagesUp()
    {
        // Given
        var command = GetAllSalesHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var sales = GetAllSalesHandlerTestData.GenerateSales(10);
        var saleResults = GetAllSalesHandlerTestData.GenerateSaleResults(sales);

        _saleRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<Guid?>(),
                Arg.Any<SaleStatus?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<CancellationToken>())
            .Returns((Sales: sales, TotalCount: 31));

        _mapper.Map<List<GetAllSalesResult>>(sales)
            .Returns(saleResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(4);
        result.TotalCount.Should().Be(31);
    }

    /// <summary>
    /// Tests that an empty repository result returns an empty paginated response.
    /// </summary>
    [Fact(DisplayName = "Given no sales When getting sales Then returns empty paginated response")]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResponse()
    {
        // Given
        var command = GetAllSalesHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var sales = new List<Sale>();
        var saleResults = new List<GetAllSalesResult>();

        _saleRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<Guid?>(),
                Arg.Any<SaleStatus?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<CancellationToken>())
            .Returns((Sales: sales, TotalCount: 0));

        _mapper.Map<List<GetAllSalesResult>>(sales)
            .Returns(saleResults);

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
    [Fact(DisplayName = "Given request without filters When getting sales Then calls repository with null filters")]
    public async Task Handle_RequestWithoutFilters_CallsRepositoryWithNullFilters()
    {
        // Given
        var command = new GetAllSalesCommand
        {
            PageNumber = 1,
            PageSize = 10,
            SaleNumber = null,
            UserId = null,
            BranchId = null,
            Status = null,
            StartDate = null,
            EndDate = null,
            MinTotalAmount = null,
            MaxTotalAmount = null
        };

        var sales = GetAllSalesHandlerTestData.GenerateSales(2);
        var saleResults = GetAllSalesHandlerTestData.GenerateSaleResults(sales);

        _saleRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<Guid?>(),
                Arg.Any<SaleStatus?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<CancellationToken>())
            .Returns((Sales: sales, TotalCount: sales.Count));

        _mapper.Map<List<GetAllSalesResult>>(sales)
            .Returns(saleResults);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetAllAsync(
            Arg.Is<int>(pageNumber => pageNumber == command.PageNumber),
            Arg.Is<int>(pageSize => pageSize == command.PageSize),
            Arg.Is<string?>(saleNumber => saleNumber == null),
            Arg.Is<Guid?>(userId => userId == null),
            Arg.Is<Guid?>(branchId => branchId == null),
            Arg.Is<SaleStatus?>(status => status == null),
            Arg.Is<DateTime?>(startDate => startDate == null),
            Arg.Is<DateTime?>(endDate => endDate == null),
            Arg.Is<decimal?>(minTotalAmount => minTotalAmount == null),
            Arg.Is<decimal?>(maxTotalAmount => maxTotalAmount == null),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get sales.
    /// </summary>
    [Fact(DisplayName = "Given valid request When repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenRepositoryFails_ThrowsException()
    {
        // Given
        var command = GetAllSalesHandlerTestData.GenerateValidCommand();

        _saleRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<Guid?>(),
                Arg.Any<SaleStatus?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<(List<Sale> Sales, int TotalCount)>>(_ =>
                throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        _mapper.DidNotReceive()
            .Map<List<GetAllSalesResult>>(Arg.Any<List<Sale>>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the mapper fails to map sales.
    /// </summary>
    [Fact(DisplayName = "Given sales from repository When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = GetAllSalesHandlerTestData.GenerateValidCommand();

        var sales = GetAllSalesHandlerTestData.GenerateSales(5);

        _saleRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<Guid?>(),
                Arg.Any<SaleStatus?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<CancellationToken>())
            .Returns((Sales: sales, TotalCount: sales.Count));

        _mapper.Map<List<GetAllSalesResult>>(sales)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _saleRepository.Received(1).GetAllAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<Guid?>(),
            Arg.Any<Guid?>(),
            Arg.Any<SaleStatus?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<CancellationToken>());
    }
}