using AutoMapper;
using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Products.Events;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using FluentValidation;


namespace SalesProject.Application.Products.CreateProduct;

/// <summary>
/// Handler for processing CreateProductCommand requests
/// </summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;

    // <summary>
    /// Initializes a new instance of CreateProductHandler
    /// </summary>
    /// <param name="productRepository">The product repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for CreateProductCommand</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public CreateProductHandler(IProductRepository productRepository, IMapper mapper, IMessageBusService messageBusService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the CreateProductCommand request
    /// </summary>
    /// <param name="command">The CreateProduct command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created product details</returns>
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateProductCommandValidator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingProduct = await _productRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingProduct != null)
            throw new InvalidOperationException($"Product with name {command.Name} already exists");

        var product = _mapper.Map<Product>(command);

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);

        await _messageBusService.PublishAsync(new ProductCreatedEvent
        {
            Id = createdProduct.Id,
            Name = createdProduct.Name,
            CurrentPrice = createdProduct.CurrentPrice,
            Status = createdProduct.Status,
            CreatedAt = createdProduct.CreatedAt,
        }, cancellationToken);


        var result = _mapper.Map<CreateProductResult>(createdProduct);
        return result;
    }
}
