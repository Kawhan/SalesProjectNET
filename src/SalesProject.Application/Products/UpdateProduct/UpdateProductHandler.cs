using AutoMapper;
using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Products.Events;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;

using FluentValidation;

namespace SalesProject.Application.Products.UpdateProduct;

/// <summary>
/// Handler for processing UpdateProductCommand requests
/// </summary>
public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;


    // <summary>
    /// Initializes a new instance of UpdateProductHandler
    /// </summary>
    /// <param name="productRepository">The product repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for UpdateProductCommand</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public UpdateProductHandler(IProductRepository productRepository, IMapper mapper, IMessageBusService messageBusService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the UpdateProductCommand request
    /// </summary>
    /// <param name="command">The UpdateProduct command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated product details</returns>
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductCommandValidator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingProduct = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingProduct is null)
            throw new KeyNotFoundException($"Product with ID {command.Id} was not exists");

        var existingProductWithDuplicateName = await _productRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingProductWithDuplicateName != null && existingProductWithDuplicateName.Id != command.Id)
            throw new InvalidOperationException($"Product with name {command.Name} already exists");

        existingProduct.Name = command.Name;
        existingProduct.CurrentPrice = command.CurrentPrice;
        existingProduct.Status = command.Status;
        existingProduct.UpdateTimestamp();

        var product = _mapper.Map<Product>(existingProduct);

        var updatedProduct = await _productRepository.UpdateAsync(product, cancellationToken);

        await _messageBusService.PublishAsync(new ProductModifiedEvent
        {
            Id = updatedProduct.Id,
            Name = updatedProduct.Name,
            CurrentPrice = updatedProduct.CurrentPrice,
            Status = updatedProduct.Status,
            CreatedAt = updatedProduct.CreatedAt,
            UpdatedAt = updatedProduct.UpdatedAt.GetValueOrDefault(DateTime.Now)
        }, cancellationToken);

        var result = _mapper.Map<UpdateProductResult>(updatedProduct);
        return result;
    }
}
