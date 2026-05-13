
using AutoMapper;
using MediatR;
using SalesProject.Domain.Repositories;

namespace SalesProject.Application.Sales.GetSale;

using FluentValidation;

/// <summary>
/// Handles the GetSaleCommand request.
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSaleCommand request.
    /// </summary>
    /// <param name="request">The GetSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale details.</returns>
    public async Task<GetSaleResult> Handle(
        GetSaleCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with id {request.Id} was not found.");

        return _mapper.Map<GetSaleResult>(sale);
    }
}
