using AutoMapper;
using MediatR;
using SalesProject.Domain.Repositories;

namespace SalesProject.Application.Products.GetAllProducts;

/// <summary>
/// Handler for processing GetAllProductsCommand requests.
/// </summary>
public class GetAllProductsHandler : IRequestHandler<GetAllProductsCommand, GetAllProductsPaginatedResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetAllProductsHandler.
    /// </summary>
    /// <param name="productRepository">The product repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetAllProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetAllProductsCommand request.
    /// </summary>
    /// <param name="request">The GetAllProducts command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paginated list of products.</returns>
    public async Task<GetAllProductsPaginatedResult> Handle(
        GetAllProductsCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _productRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            request.Name,
            request.MinPrice,
            request.MaxPrice,
            request.Status,
            cancellationToken);

        var products = _mapper.Map<List<GetAllProductsResult>>(result.Products);

        return new GetAllProductsPaginatedResult
        {
            Data = products,
            CurrentPage = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)request.PageSize)
        };
    }
}

