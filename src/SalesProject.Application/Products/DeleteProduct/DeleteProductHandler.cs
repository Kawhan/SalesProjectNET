using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Products.Events;
using SalesProject.Domain.Repositories;
using FluentValidation;

namespace SalesProject.Application.Products.DeleteProduct;

/// <summary>
/// Handler for processing DeleteProductCommand requests
/// </summary>
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IMessageBusService _messageBusService;

    /// <summary>
    /// Initializes a new instance of DeleteProductHandler
    /// </summary>
    /// <param name="productRepository">The product repository</param>
    /// <param name="validator">The validator for DeleteProductCommand</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public DeleteProductHandler(
        IProductRepository productRepository, IMessageBusService messageBusService)
    {
        _productRepository = productRepository;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the DeleteProductCommand request
    /// </summary>
    /// <param name="request">The DeleteProduct command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteProductResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteProductValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _productRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"Product with ID {request.Id} not found");

        var deletedProduct = await _productRepository.GetByIdAsync(request.Id);

        await _messageBusService.PublishAsync(new ProductDeletedEvent
        {
            Id = deletedProduct.Id,
            Name = deletedProduct.Name,
            CurrentPrice = deletedProduct.CurrentPrice,
            Status = deletedProduct.Status,
            CreatedAt = deletedProduct.CreatedAt,
            UpdatedAt = deletedProduct.UpdatedAt.GetValueOrDefault(DateTime.Now)
        }, cancellationToken);

        return new DeleteProductResponse { Success = true };
    }
}

