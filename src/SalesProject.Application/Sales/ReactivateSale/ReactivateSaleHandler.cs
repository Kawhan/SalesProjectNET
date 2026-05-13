using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Sales.Events;
using SalesProject.Domain.Repositories;

using FluentValidation;

namespace SalesProject.Application.Sales.ReactivateSale;

/// <summary>
/// Handler for processing ReactivateSaleCommand requests.
/// </summary>
public class ReactivateSaleHandler : IRequestHandler<ReactivateSaleCommand, ReactivateSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMessageBusService _messageBusService;


    /// <summary>
    /// Initializes a new instance of ReactivateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository instance</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public ReactivateSaleHandler(ISaleRepository saleRepository, IMessageBusService messageBusService)
    {
        _saleRepository = saleRepository;
        _messageBusService = messageBusService;
    }

    public async Task<ReactivateSaleResponse> Handle(
        ReactivateSaleCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new ReactivateSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _saleRepository.ReactivateAsync(request.Id, cancellationToken);

        if (!success)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");

        var reactivatedSale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (reactivatedSale is null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");

        await _messageBusService.PublishAsync(new SaleReactivatedEvent
        {
            SaleId = reactivatedSale.Id,
            SaleNumber = reactivatedSale.SaleNumber,
            UserId = reactivatedSale.UserId,
            BranchId = reactivatedSale.BranchId,
            TotalAmount = reactivatedSale.TotalAmount,
            CreatedAt = reactivatedSale.CreatedAt,
            UpdatedAt = reactivatedSale.UpdatedAt.GetValueOrDefault(DateTime.UtcNow),
        }, cancellationToken);


        return new ReactivateSaleResponse
        {
            Success = true
        };
    }
}