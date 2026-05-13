using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Sales.Events;
using SalesProject.Domain.Repositories;

using FluentValidation;

namespace SalesProject.Application.Sales.DeleteSale;

/// <summary>
/// Handler for processing DeleteSaleCommand requests
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMessageBusService _messageBusService;

    /// <summary>
    /// Initializes a new instance of DeleteSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public DeleteSaleHandler(ISaleRepository saleRepository, IMessageBusService messageBusService)
    {
        _saleRepository = saleRepository;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the DeleteSaleCommand request
    /// </summary>
    /// <param name="request">The DeleteSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteSaleResponse> Handle(
        DeleteSaleCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _saleRepository.DeleteAsync(request.Id, cancellationToken);

        if (!success)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        await _messageBusService.PublishAsync(new SaleCancelledEvent
        {
            SaleId = request.Id,
            SaleNumber = sale.SaleNumber,
            UserId = sale.UserId,
            BranchId = sale.BranchId,
            TotalAmount = sale.TotalAmount,
            CreatedAt = sale.CreatedAt,
            CancelledAt = DateTime.UtcNow
        }, cancellationToken);


        return new DeleteSaleResponse
        {
            Success = true
        };
    }
}
