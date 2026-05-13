using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Products.GetAllProducts;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Products.TestData;

namespace SalesProject.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="GetAllProductsHandler"/> class.
/// </summary>
public class GetAllProductsHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetAllProductsHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllProductsHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetAllProductsHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetAllProductsHandler(_productRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get all products request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting products Then returns paginated response")]
    public async Task Handle_ValidRequest_ReturnsPaginatedResponse()
    {
        // Given
        var command = GetAllProductsHandlerTestData.GenerateValidCommand();
        command.PageNumber = 2;
        command.PageSize = 10;

        var products = GetAllProductsHandlerTestData.GenerateProducts(10);
        var productResults = GetAllProductsHandlerTestData.GenerateProductResults(products);

        _productRepository.GetAllAsync(
                command.PageNumber,
                command.PageSize,
                command.Name,
                command.MinPrice,
                command.MaxPrice,
                command.Status,
                Arg.Any<CancellationToken>())
            .Returns((Products: products, TotalCount: 25));

        _mapper.Map<List<GetAllProductsResult>>(products)
            .Returns(productResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(10);
        result.Data.Should().BeEquivalentTo(productResults);
        result.CurrentPage.Should().Be(command.PageNumber);
        result.PageSize.Should().Be(command.PageSize);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
    }

    /// <summary>
    /// Tests that the repository is called with the correct pagination and filter parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting products Then calls repository with correct filters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectFilters()
    {
        // Given
        var command = GetAllProductsHandlerTestData.GenerateValidCommand();

        var products = GetAllProductsHandlerTestData.GenerateProducts(3);
        var productResults = GetAllProductsHandlerTestData.GenerateProductResults(products);

        _productRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<ProductStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Products: products, TotalCount: products.Count));

        _mapper.Map<List<GetAllProductsResult>>(products)
            .Returns(productResults);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).GetAllAsync(
            Arg.Is<int>(pageNumber => pageNumber == command.PageNumber),
            Arg.Is<int>(pageSize => pageSize == command.PageSize),
            Arg.Is<string?>(name => name == command.Name),
            Arg.Is<decimal?>(minPrice => minPrice == command.MinPrice),
            Arg.Is<decimal?>(maxPrice => maxPrice == command.MaxPrice),
            Arg.Is<ProductStatus?>(status => status == command.Status),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that products returned by the repository are mapped to result objects.
    /// </summary>
    [Fact(DisplayName = "Given products from repository When getting products Then maps products to result")]
    public async Task Handle_ValidRequest_MapsProductsToResult()
    {
        // Given
        var command = GetAllProductsHandlerTestData.GenerateValidCommand();

        var products = GetAllProductsHandlerTestData.GenerateProducts(5);
        var productResults = GetAllProductsHandlerTestData.GenerateProductResults(products);

        _productRepository.GetAllAsync(
                command.PageNumber,
                command.PageSize,
                command.Name,
                command.MinPrice,
                command.MaxPrice,
                command.Status,
                Arg.Any<CancellationToken>())
            .Returns((Products: products, TotalCount: products.Count));

        _mapper.Map<List<GetAllProductsResult>>(products)
            .Returns(productResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<List<GetAllProductsResult>>(
            Arg.Is<List<Product>>(p => p == products));

        result.Data.Should().BeEquivalentTo(productResults);
    }

    /// <summary>
    /// Tests that total pages are calculated correctly when total count is divisible by page size.
    /// </summary>
    [Fact(DisplayName = "Given total count divisible by page size When getting products Then calculates total pages correctly")]
    public async Task Handle_ValidRequest_WhenTotalCountIsDivisibleByPageSize_CalculatesTotalPages()
    {
        // Given
        var command = GetAllProductsHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var products = GetAllProductsHandlerTestData.GenerateProducts(10);
        var productResults = GetAllProductsHandlerTestData.GenerateProductResults(products);

        _productRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<ProductStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Products: products, TotalCount: 30));

        _mapper.Map<List<GetAllProductsResult>>(products)
            .Returns(productResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(3);
        result.TotalCount.Should().Be(30);
    }

    /// <summary>
    /// Tests that total pages are rounded up when total count is not divisible by page size.
    /// </summary>
    [Fact(DisplayName = "Given total count not divisible by page size When getting products Then rounds total pages up")]
    public async Task Handle_ValidRequest_WhenTotalCountIsNotDivisibleByPageSize_RoundsTotalPagesUp()
    {
        // Given
        var command = GetAllProductsHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var products = GetAllProductsHandlerTestData.GenerateProducts(10);
        var productResults = GetAllProductsHandlerTestData.GenerateProductResults(products);

        _productRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<ProductStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Products: products, TotalCount: 31));

        _mapper.Map<List<GetAllProductsResult>>(products)
            .Returns(productResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(4);
        result.TotalCount.Should().Be(31);
    }

    /// <summary>
    /// Tests that an empty repository result returns an empty paginated response.
    /// </summary>
    [Fact(DisplayName = "Given no products When getting products Then returns empty paginated response")]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResponse()
    {
        // Given
        var command = GetAllProductsHandlerTestData.GenerateValidCommand();
        command.PageNumber = 1;
        command.PageSize = 10;

        var products = new List<Product>();
        var productResults = new List<GetAllProductsResult>();

        _productRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<ProductStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Products: products, TotalCount: 0));

        _mapper.Map<List<GetAllProductsResult>>(products)
            .Returns(productResults);

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
    [Fact(DisplayName = "Given request without filters When getting products Then calls repository with null filters")]
    public async Task Handle_RequestWithoutFilters_CallsRepositoryWithNullFilters()
    {
        // Given
        var command = new GetAllProductsCommand
        {
            PageNumber = 1,
            PageSize = 10,
            Name = null,
            MinPrice = null,
            MaxPrice = null,
            Status = null
        };

        var products = GetAllProductsHandlerTestData.GenerateProducts(2);
        var productResults = GetAllProductsHandlerTestData.GenerateProductResults(products);

        _productRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<ProductStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Products: products, TotalCount: products.Count));

        _mapper.Map<List<GetAllProductsResult>>(products)
            .Returns(productResults);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).GetAllAsync(
            Arg.Is<int>(pageNumber => pageNumber == command.PageNumber),
            Arg.Is<int>(pageSize => pageSize == command.PageSize),
            Arg.Is<string?>(name => name == null),
            Arg.Is<decimal?>(minPrice => minPrice == null),
            Arg.Is<decimal?>(maxPrice => maxPrice == null),
            Arg.Is<ProductStatus?>(status => status == null),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get products.
    /// </summary>
    [Fact(DisplayName = "Given valid request When repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenRepositoryFails_ThrowsException()
    {
        // Given
        var command = GetAllProductsHandlerTestData.GenerateValidCommand();

        _productRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<ProductStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<(List<Product> Products, int TotalCount)>>(_ =>
                throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        _mapper.DidNotReceive()
            .Map<List<GetAllProductsResult>>(Arg.Any<List<Product>>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the mapper fails to map products.
    /// </summary>
    [Fact(DisplayName = "Given products from repository When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = GetAllProductsHandlerTestData.GenerateValidCommand();

        var products = GetAllProductsHandlerTestData.GenerateProducts(5);

        _productRepository.GetAllAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<ProductStatus?>(),
                Arg.Any<CancellationToken>())
            .Returns((Products: products, TotalCount: products.Count));

        _mapper.Map<List<GetAllProductsResult>>(products)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _productRepository.Received(1).GetAllAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<ProductStatus?>(),
            Arg.Any<CancellationToken>());
    }
}