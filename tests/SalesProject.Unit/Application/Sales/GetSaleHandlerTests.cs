using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Sales.GetSale;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Sales.TestData;

namespace SalesProject.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get sale request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When getting sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        var sale = GetSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        var result = GetSaleHandlerTestData.GenerateSaleResult(sale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale)
            .Returns(result);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getSaleResult.Should().NotBeNull();
        getSaleResult.Id.Should().Be(sale.Id);
        getSaleResult.SaleNumber.Should().Be(sale.SaleNumber);
        getSaleResult.UserId.Should().Be(sale.UserId);
        getSaleResult.BranchId.Should().Be(sale.BranchId);
        getSaleResult.TotalAmount.Should().Be(sale.TotalAmount);
        getSaleResult.Status.Should().Be(sale.Status);
        getSaleResult.Items.Should().HaveCount(sale.Items.Count);
    }

    /// <summary>
    /// Tests that an invalid get sale request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale id When getting sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new GetSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale id When getting sale Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new GetSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _saleRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<GetSaleResult>(Arg.Any<Sale>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the sale does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing sale id When getting sale Then throws key not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with id {command.Id} was not found.");

        await _saleRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        _mapper.DidNotReceive()
            .Map<GetSaleResult>(Arg.Any<Sale>());
    }

    /// <summary>
    /// Tests that the repository is called with the correct sale id.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When getting sale Then calls repository with correct id")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        var sale = GetSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        var result = GetSaleHandlerTestData.GenerateSaleResult(sale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale)
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1)
            .GetByIdAsync(
                Arg.Is<Guid>(id => id == command.Id),
                Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the sale returned by repository.
    /// </summary>
    [Fact(DisplayName = "Given existing sale When getting sale Then maps sale to result")]
    public async Task Handle_ValidRequest_MapsSaleToResult()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        var sale = GetSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        var result = GetSaleHandlerTestData.GenerateSaleResult(sale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale)
            .Returns(result);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1)
            .Map<GetSaleResult>(Arg.Is<Sale>(s =>
                s.Id == sale.Id &&
                s.SaleNumber == sale.SaleNumber &&
                s.UserId == sale.UserId &&
                s.BranchId == sale.BranchId &&
                s.TotalAmount == sale.TotalAmount &&
                s.Status == sale.Status));

        getSaleResult.Should().BeEquivalentTo(result);
    }

    /// <summary>
    /// Tests that nested sale data is returned in the result.
    /// </summary>
    [Fact(DisplayName = "Given existing sale with user branch and items When getting sale Then returns nested data")]
    public async Task Handle_ValidRequest_ReturnsNestedSaleData()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        var sale = GetSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        var result = GetSaleHandlerTestData.GenerateSaleResult(sale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale)
            .Returns(result);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getSaleResult.User.Should().NotBeNull();
        getSaleResult.User.Id.Should().Be(sale.User.Id);
        getSaleResult.User.Username.Should().Be(sale.User.Username);
        getSaleResult.User.Email.Should().Be(sale.User.Email);

        getSaleResult.Branch.Should().NotBeNull();
        getSaleResult.Branch.Id.Should().Be(sale.Branch.Id);
        getSaleResult.Branch.Name.Should().Be(sale.Branch.Name);
        getSaleResult.Branch.Address.Should().Be(sale.Branch.Address);
        getSaleResult.Branch.Status.Should().Be(sale.Branch.Status);

        getSaleResult.Items.Should().NotBeEmpty();
        getSaleResult.Items[0].Product.Should().NotBeNull();
        getSaleResult.Items[0].ProductId.Should().Be(sale.Items[0].ProductId);
        getSaleResult.Items[0].Quantity.Should().Be(sale.Items[0].Quantity);
    }

    /// <summary>
    /// Tests that an exception is thrown when the repository fails to get the sale.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When repository fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenRepositoryFails_ThrowsException()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns<Task<Sale?>>(_ => throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        _mapper.DidNotReceive()
            .Map<GetSaleResult>(Arg.Any<Sale>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the mapper fails to map the sale.
    /// </summary>
    [Fact(DisplayName = "Given existing sale When mapper fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMapperFails_ThrowsException()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        var sale = GetSaleHandlerTestData.GenerateSale();
        sale.Id = command.Id;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale)
            .Returns(_ => throw new AutoMapperMappingException("Mapping error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<AutoMapperMappingException>()
            .WithMessage("Mapping error");

        await _saleRepository.Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
}
